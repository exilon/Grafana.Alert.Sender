using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAlertGenerator
    {
       public string GenerateAlert(Alert alert);
       public Task<Stream> GenerateImage(Alert alert);
    }
}
