using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desarrollo_web_ASP.NET_Core_Practica_3.Pages
{
    public class CajeroAutomaticoModel : PageModel
    {
        public Cajero cajero { get; set; } = Cajero.Instance;
        public ErrorMessage errorMessage { get; set; } 
        public List<Billete> billetes { get; set; } 
        public CajeroAutomaticoModel()
        {
            this.errorMessage = new ErrorMessage();       
        }

        public void OnGet(int monto, string banco)
        {

            if(banco == "ABC")
            {
                RealizarTransaccion(monto, 10000);
            }
            else
            {
                RealizarTransaccion(monto, 2000);
            }
        }

        private int GetTotalDineroCajero()
        {
            return cajero.billetes.Sum(b => b.NumeroBillete * b.Cantidad);
        }

        private void RealizarTransaccion(int monto, int limiteRetiro)
        {
            if (GetTotalDineroCajero() == 0)
            {
                SetError(true, "No hay dinero disponible");
            }
            else if (monto > limiteRetiro)
            {
                SetError(true, "El monto digitado excede el límite de transacción");
            }
            else if (monto > GetTotalDineroCajero())
            {
                SetError(true, "El monto solicitado no puede ser dispensado");
            }
            else
            {
                SetDistribucionBilletes(monto);
            }
           
        }

        private void SetDistribucionBilletes(int monto)
        {
            billetes = new List<Billete>();
            int countMiles = monto / 1000;
            int countQuinientos = (monto - (countMiles * 1000)) / 500;
            int countCienes = (monto - ((countMiles * 1000) + (countQuinientos * 500))) / 100;
            int total = (countMiles * 1000) + (countQuinientos * 500) + (countCienes * 100);

            if (total != monto)
            {
                SetError(true, "El monto solicitado no puede ser dispensado");
                return;
            }

            if (countMiles > 0)
            {
                SetBilleteMiles(countMiles);               
            }

            if (countQuinientos > 0 )
            {
                SetBilleteQuinientos(countQuinientos);
            }
            if (countCienes > 0 )
            {
                SetBilleteCienes(countCienes);
            }
            UpdateCajero();

        }

        private void SetBilleteMiles(int countMiles)
        {
            if (countMiles <= cajero.billetes[0].Cantidad)
            {
                billetes.Add(new Billete { NumeroBillete = 1000, Cantidad = countMiles });
            }
            else
            {
                int mil = countMiles - cajero.billetes[0].Cantidad;
                int dineroSobranteQuinientos = ((countMiles - cajero.billetes[0].Cantidad) * 1000) / 500;
                billetes.Add(new Billete { NumeroBillete = 1000, Cantidad = countMiles - mil});
                SetBilleteQuinientos(dineroSobranteQuinientos);
            }
        }

        private void SetBilleteQuinientos(int countQuinientos)
        {
            if (countQuinientos <= cajero.billetes[1].Cantidad)
            {
                billetes.Add(new Billete { NumeroBillete = 500, Cantidad = countQuinientos });
            }
            else
            {
                int quiniento = countQuinientos - cajero.billetes[1].Cantidad;
                int dineroSobranteCienes = ((countQuinientos - cajero.billetes[1].Cantidad) * 500) / 100;
                billetes.Add(new Billete { NumeroBillete = 500, Cantidad = countQuinientos - quiniento });
                SetBilleteCienes(dineroSobranteCienes);
            }
        }

        private void SetBilleteCienes(int countCienes)
        {
            if (countCienes <= cajero.billetes[2].Cantidad)
            {
                billetes.Add(new Billete { NumeroBillete = 100, Cantidad = countCienes });
            }
            else
            {
                SetError(true, "El monto solicitado no puede ser dispensado, ya que el cajero no dispone de esea cantidad");
            }
           
        }

        private void SetError(bool IsError, string Message)
        {
            errorMessage.IsError = IsError;
            errorMessage.Message = Message;
        }

        private void UpdateCajero()
        {
            foreach (var billetesCajeto in cajero.billetes)
            {
                foreach (var billetesUsuario in billetes)
                {
                    if(billetesCajeto.NumeroBillete == billetesUsuario.NumeroBillete)
                    {
                        billetesCajeto.Cantidad -= billetesUsuario.Cantidad;
                    }
                }
            }
        }
    }

    public class Cajero
    {
        public List<Banco> bancos { get; set; }
        public List<Billete> billetes { get; set; }

        private readonly static Cajero _instance = new Cajero();

        private Cajero()
        {
            bancos = new List<Banco>
            {
                    new Banco { NombreBanco = "ABC" },
                    new Banco { NombreBanco = "Banco BHD" },
                    new Banco { NombreBanco = "Banreservas" },
                    new Banco { NombreBanco = "Banco Popular" },
                    new Banco { NombreBanco = "Banco León" },
            };
            billetes = new List<Billete>
            {
                    new Billete {NumeroBillete = 1000, Cantidad = 9 },
                    new Billete {NumeroBillete = 500, Cantidad = 19 },
                    new Billete {NumeroBillete = 100, Cantidad = 99 },
            };
        }

        public static Cajero Instance
        {
            get
            {
                return _instance;
            }
        }
    }

    public class Banco
    {
        public string NombreBanco { get; set; }
    }

    public class Billete
    {
        public int NumeroBillete { get; set; }
        public int Cantidad { get; set; }
    }

    public class ErrorMessage
    {
        public bool IsError { get; set; } = false;
        public string Message { get; set; }
    }
}
