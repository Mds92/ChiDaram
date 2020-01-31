using ChiDaram.Common.Enums;

namespace ChiDaram.Common.Entity
{
    public class Setting
    {
        public Setting()
        {
            Value = "";
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public SettingEnum TitleEnum { get; set; }
        public string Value { get; set; }
    }
}
