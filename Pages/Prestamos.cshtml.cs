using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desarrollo_web_ASP.NET_Core_Practica_3.Pages
{
    public class PrestamosModel : PageModel
    {
        public double monto { get; set; }
        public int CantidadCuotas { get; set; }
        public double porcentajeAnual { get; set; }
        public double cuotaMensual { get; set; }

        public void OnGet(double monto, int cuotas, double porcentaje)
        {
            this.monto = monto;
            this.CantidadCuotas = cuotas;
            this.porcentajeAnual = porcentaje;
            this.cuotaMensual = GetCuotaMensual();
        }

        private double GetCuotaMensual()
        {
            double tasa = this.porcentajeAnual / 100;
            double cuota = Math.Round((tasa * this.monto) / (1 - Math.Pow((double)(1+ tasa), -this.CantidadCuotas) ), 2);
            return cuota;
        }
    }
}
