using ChiDaram.Common.Entity;

namespace ChiDaram.Api.Classes.Security
{
    public class SecurityContextMdsCms
    {
        public User User { get; set; }
        public string ConnectionId { get; set; }
    }
}
