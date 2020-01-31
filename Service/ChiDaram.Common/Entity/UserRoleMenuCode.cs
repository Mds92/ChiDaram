using ChiDaram.Common.Enums;

namespace ChiDaram.Common.Entity
{
    public class UserRoleMenuCode
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public UserRoleMenuCodeEnum MenuCode { get; set; }
        public UserRoleMenuCodeFunctionIdEnum FunctionId { get; set; }
    }
}
