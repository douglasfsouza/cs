using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3
{
    public class Radar
    {
        public delegate bool VelocidadeExcedida(string placa, int velocidade, string via);
        public event VelocidadeExcedida EventoGerarMulta;
        public bool isMovel { get; set; }
        public int LimiteVelocidadePermitida { get; set; }
        public string via { get; set; }

        public virtual void Multar(string placa, int km, string via)
        {
            EventoGerarMulta(placa, km, via);
        }
        public bool ValidarVelocidade(int km, string placa, string via)
        {
            if (km > LimiteVelocidadePermitida)
            {
                Multar(placa, km, via);
                return false;
            }
            else
                return true;
        }
    }
        
}
