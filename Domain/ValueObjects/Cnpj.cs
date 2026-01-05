namespace Domain.ValueObjects
{
    public class Cnpj : IEquatable<Cnpj>
    {
        public string Valor { get; private set; }

        private Cnpj(string cnpj)
        {
            Valor = cnpj;
        }

        public static Cnpj Criar(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("CNPJ não pode ser vazio", nameof(cnpj));

            var cnpjLimpo = RemoverFormatacao(cnpj);

            if (!Validar(cnpjLimpo))
                throw new ArgumentException("CNPJ inválido", nameof(cnpj));

            return new Cnpj(cnpjLimpo);
        }
        
        public static Cnpj? TentarCriar(string cnpj)
        {
            try
            {
                return Criar(cnpj);
            }
            catch
            {
                return null;
            }
        }

        public string Formatado()
        {
            if (Valor.Length != 14)
                return Valor;

            return $"{Valor.Substring(0, 2)}.{Valor.Substring(2, 3)}.{Valor.Substring(5, 3)}/{Valor.Substring(8, 4)}-{Valor.Substring(12, 2)}";
        }

      
        public override string ToString() => Valor;


        private static string RemoverFormatacao(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private static bool Validar(string cnpj)
        {
            if (cnpj.Length != 14)
                return false;

            if (cnpj.Distinct().Count() == 1)
                return false;

            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += (cnpj[i] - '0') * multiplicadores1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += (cnpj[i] - '0') * multiplicadores2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj[12] - '0' == digito1 && cnpj[13] - '0' == digito2;
        }


        public bool Equals(Cnpj? other)
        {
            if (other is null) return false;
            return Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cnpj);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

        public static bool operator ==(Cnpj? left, Cnpj? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Cnpj? left, Cnpj? right)
        {
            return !(left == right);
        }

        public static implicit operator Cnpj(string cnpj)
        {
            return Criar(cnpj);
        }

        public static implicit operator string(Cnpj cnpj)
        {
            return cnpj?.Valor ?? string.Empty;
        }
    }
}