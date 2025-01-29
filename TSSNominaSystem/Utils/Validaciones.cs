namespace TSSNominaSystem.Utils
{
    public static class Validaciones
    {
        public static bool ValidarCedula(string cedula)
        {
            if (cedula.Length != 11 || !cedula.All(char.IsDigit)) return false;
            int suma = 0;
            for (int i = 0; i < 10; i++)
            {
                int num = int.Parse(cedula[i].ToString());
                suma += (i % 2 == 0) ? num * 1 : ((num * 2 > 9) ? num * 2 - 9 : num * 2);
            }
            int digitoVerificador = (10 - (suma % 10)) % 10;
            return digitoVerificador == int.Parse(cedula[10].ToString());
        }
    }
}
