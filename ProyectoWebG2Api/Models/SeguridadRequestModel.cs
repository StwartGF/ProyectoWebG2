// Models/SeguridadRequestModel.cs
using System.ComponentModel.DataAnnotations;
namespace SM_ProyectoAPI.Models
{
    public class SeguridadRequestModel
    {
        [Required] public int ConsecutivoUsuario { get; set; }
        [Required] public string Contrasenna { get; set; } = "";
    }
}