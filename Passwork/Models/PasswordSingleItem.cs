namespace Passwork.Models
{
    class PasswordSingleItem : PasswordListItem
    {
        public AttachmentListItem[] attachments { get; set; }
        public PasswordCustomItem[] custom { get; set; }
        public string cryptedPassword { get; set; }

    }

}
