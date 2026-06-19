using System.Collections.ObjectModel;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Permissions
{
    public class UserPermissions
    {
        private readonly List<UserPermissionEnum> _permissions = new List<UserPermissionEnum>();
        public IReadOnlyCollection<UserPermissionEnum> PermissionsList
        {
            get { return new ReadOnlyCollection<UserPermissionEnum>(_permissions); }
        }

        public RolePermissions RolePermissions
        {
            get
            {
                return new RolePermissions(_permissions);
            }
        }

        public void AddPermission(UserPermissionEnum permission)
        {
            _permissions.Add(permission);
        }

        public void AddPermissions(IEnumerable<UserPermissionEnum> permissions)
        {
            _permissions.AddRange(permissions);
        }

    }

    public class RolePermissions
    {
        private readonly List<UserPermissionEnum> _permissions;

        public RolePermissions(List<UserPermissionEnum> permissions)
        {
            _permissions = permissions;
        }

        public bool IsAdmin { get { return _permissions.Contains(UserPermissionEnum.Admin); } }
    }
}
