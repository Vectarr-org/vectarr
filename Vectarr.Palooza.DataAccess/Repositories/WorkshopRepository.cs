using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopRepository : BaseRepository<Workshop, WorkshopModel>, IWorkshopRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkshopRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<Workshop, WorkshopModel>> GetEntityToModelExpression()
        {
            var addressExpression = _unitOfWork.Repositories.AddressRepository.GetEntityToModelExpression().Expand();
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var workshopCapabilitiesExpression = _unitOfWork.Repositories.WorkshopCapabilityRepository.GetEntityToModelExpression().Expand();
            var workshopSpecializationsExpression = _unitOfWork.Repositories.WorkshopSpecializationRepository.GetEntityToModelExpression().Expand();
            var workshopEquipmentExpression = _unitOfWork.Repositories.WorkshopEquipmentRepository.GetEntityToModelExpression().Expand();
            var workshopCertificatesExpression = _unitOfWork.Repositories.WorkshopCertificationRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkshopModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address != null ? addressExpression.Invoke(entity.Address) : null,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                WebsiteUrl = entity.WebsiteUrl,
                CompanyInfo = entity.CompanyInfo,
                CompanyName = entity.CompanyName,
                Logo = entity.Logo != null ? imageExpression.Invoke(entity.Logo) : null,
                Description = entity.Description,
                DateCreated = entity.CreatedDate,
                WorkshopCapabilities = entity.WorkshopCapabilities.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden)
                .Select(workshopCapabilitiesExpression).ToList(),
                WorkshopSpecializations = entity.WorkshopSpecializations.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden)
                .Select(workshopSpecializationsExpression).ToList(),
                WorkshopEquipment = entity.WorkshopEquipment.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden)
                .Select(workshopEquipmentExpression).ToList(),
                WorkshopCertifications = entity.WorkshopCertifications.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden)
                .Select(workshopCertificatesExpression).ToList(),
                Images = entity.Images.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden).Select(imageExpression).ToList(),
                Country = entity.Country ?? (entity.Address != null ? entity.Address.Country : null),
            };
        }

        public override IQueryable<Workshop> GetIncludes(IQueryable<Workshop> query)
        {
            return query.Include(s => s.WorkshopCapabilities)
                .Include(s => s.WorkshopSpecializations)
                .Include(s => s.WorkshopEquipment)
                .Include(s => s.WorkshopCertifications).Include(S => S.Images);
        }

        protected override Workshop ModelToEntityInternal(WorkshopModel model, Workshop entity)
        {
            entity.WebsiteUrl = model.WebsiteUrl;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.Name = model.Name;
            entity.CompanyInfo = model.CompanyInfo;
            entity.CompanyName = model.CompanyName;
            entity.Description = model.Description;

            var resolvedCountry = SourcingRegionPolicy.ResolveCountry(model.Country, model.CountryCodeInput);
            if (!resolvedCountry.HasValue && model.Address?.Country != null)
            {
                resolvedCountry = model.Address.Country;
            }

            if (resolvedCountry.HasValue)
            {
                entity.Country = resolvedCountry;
            }

            if (model.Address != null)
            {
                if (model.Address.Id != 0)
                {
                    // Get existing address and UPDATE it with new values from model
                    var existingAddress = _unitOfWork.Repositories.AddressRepository.GetEntityById(model.Address.Id);
                    entity.Address = _unitOfWork.Repositories.AddressRepository.ModelToEntity(model.Address, existingAddress);
                }
                else
                {
                    // Create new address
                    entity.Address = _unitOfWork.Repositories.AddressRepository.ModelToEntity(model.Address, new Address());
                }
            }
            else
            {
                entity.Address = null;
            }

            if (resolvedCountry.HasValue && entity.Address != null)
            {
                entity.Address.Country = resolvedCountry;
            }
            else if (entity.Country == null && entity.Address?.Country != null)
            {
                entity.Country = entity.Address.Country;
            }

            if (model.Logo?.Id != null)
            {
                entity.Logo = _unitOfWork.Repositories.ImageRepository.GetEntityById(model.Logo.Id);
                entity.LogoId = entity.Logo?.Id;
            }
            else
            {
                entity.Logo = null;
                entity.LogoId = null;  // This will now properly clear the FK
            }

            var existingWorkshopCertifications = entity.WorkshopCertifications ?? new List<WorkshopCertification>();
            var updatedWorkshopCertifications = new List<WorkshopCertification>();
            if (model.WorkshopCertifications != null)
            {
                foreach (var certification in model.WorkshopCertifications.ToList())
                {
                    var existingCertification = existingWorkshopCertifications.FirstOrDefault(x => x.Id == certification.Id) ?? new WorkshopCertification();
                    existingCertification = UnitOfWork.Repositories.WorkshopCertificationRepository.ModelToEntity(certification, existingCertification);
                    updatedWorkshopCertifications.Add(existingCertification);
                }
            }
            entity.WorkshopCertifications = _unitOfWork.Repositories.WorkshopCertificationRepository.ReplaceCollection(existingWorkshopCertifications, updatedWorkshopCertifications);

            var existingWorkshopCapabilities = entity.WorkshopCapabilities ?? new List<WorkshopCapability>();
            var updatedWorkshopCapabilities = new List<WorkshopCapability>();
            if (model.WorkshopCapabilities != null)
            {
                foreach (var capability in model.WorkshopCapabilities.ToList())
                {
                    var existingCapability = existingWorkshopCapabilities.FirstOrDefault(x => x.Id == capability.Id) ?? new WorkshopCapability();
                    existingCapability = UnitOfWork.Repositories.WorkshopCapabilityRepository.ModelToEntity(capability, existingCapability);
                    updatedWorkshopCapabilities.Add(existingCapability);
                }
            }
            entity.WorkshopCapabilities = _unitOfWork.Repositories.WorkshopCapabilityRepository.ReplaceCollection(existingWorkshopCapabilities, updatedWorkshopCapabilities);

            var existingWorkshopSpecializations = entity.WorkshopSpecializations ?? new List<WorkshopSpecialization>();
            var updatedWorkshopSpecializations = new List<WorkshopSpecialization>();
            if (model.WorkshopSpecializations != null)
            {
                foreach (var specialization in model.WorkshopSpecializations.ToList())
                {
                    var existingSpecialization = existingWorkshopSpecializations.FirstOrDefault(x => x.Id == specialization.Id) ?? new WorkshopSpecialization();
                    existingSpecialization = UnitOfWork.Repositories.WorkshopSpecializationRepository.ModelToEntity(specialization, existingSpecialization);
                    updatedWorkshopSpecializations.Add(existingSpecialization);
                }
            }
            entity.WorkshopSpecializations = _unitOfWork.Repositories.WorkshopSpecializationRepository.ReplaceCollection(existingWorkshopSpecializations, updatedWorkshopSpecializations);

            var existingWorkshopEquipments = entity.WorkshopEquipment ?? new List<WorkshopEquipment>();
            var updatedWorkshopEquipments = new List<WorkshopEquipment>();
            if (model.WorkshopEquipment != null)
            {
                foreach (var equipment in model.WorkshopEquipment.ToList())
                {
                    var existingEquipment = existingWorkshopEquipments.FirstOrDefault(x => x.Id == equipment.Id) ?? new WorkshopEquipment();
                    existingEquipment = UnitOfWork.Repositories.WorkshopEquipmentRepository.ModelToEntity(equipment, existingEquipment);
                    updatedWorkshopEquipments.Add(existingEquipment);
                }
            }
            entity.WorkshopEquipment = _unitOfWork.Repositories.WorkshopEquipmentRepository.ReplaceCollection(existingWorkshopEquipments, updatedWorkshopEquipments);

            var existingImages = entity.Images ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.Images != null)
            {
                foreach (var image in model.Images.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    //existingImage = UnitOfWork.Repositories.ImageRepository.ModelToEntity(image, existingImage);
                    existingImage.WorkshopId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.Images = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            return entity;
        }

     
    }
}
