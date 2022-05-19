using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}