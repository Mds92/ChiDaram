using System;
using System.Collections.Generic;
using System.Linq;
using ChiDaram.Common.Enums;

namespace ChiDaram.Common.Entity
{
    public class User
    {
        public User()
        {
            MenuCodes = new List<UserRoleMenuCode>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public LanguageEnum LanguageId { get; set; }
        public UserStatusEnum StatusId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserRoleTitle { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string SecretKey { get; set; }
        public bool IsOtpPasswordEnabled { get; set; }
        public bool IsActive { get; set; }
        
        public DateTime LastLoginDateTime { get; set; }
        public DateTime RegisterDateTime { get; set; }

        public List<UserRoleMenuCode> MenuCodes { get; set; }

        /// <summary>
        /// آیا دسترسی به کنترل پنل دارد یا خیر
        /// </summary>
        public bool HasAccessToCp
        {
            get
            {
                if (_hasAccessToCp.HasValue) return _hasAccessToCp.Value;
                _hasAccessToCp = MenuCodes.Any(q => q.FunctionId != UserRoleMenuCodeFunctionIdEnum.None);
                return _hasAccessToCp.Value;
            }
        }
        private bool? _hasAccessToCp;

    }
}
