// Models/ValidarSesionRequestModel.cs
using System.ComponentModel.DataAnnotations;
namespace SM_ProyectoAPI.Models
{
    public class ValidarSesionRequestModel
    {
        [Required, EmailAddress] public string CorreoElectronico { get; set; } = "";
        [Required] public string Contrasenna { get; set; } = "";
    }
}