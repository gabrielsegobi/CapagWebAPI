//namespace Domain.Entities.Sped
//{
//    public struct ValueBuffer
//    {
//        private readonly object?[] _values;

//        public int FieldCount { get; }

//        public const int COLUNAS_FIXAS = 7;

//        public ValueBuffer(int fieldCount)
//        {
//            FieldCount = fieldCount;
//            _values = new object?[fieldCount];
//        }

//        public void Set(int index, object? value)
//        {
//            _values[index] = value;
//        }

//        public object? Get(int index) => _values[index];

//        /// <summary>
//        /// Exposição do array interno para o DataTable/BulkCopy.
//        /// Não modificar externamente.
//        /// </summary>
//        public object?[] Values => _values;

//        // Atalhos para os campos fixos — evita magic numbers no código
//        public long Id => (long)(_values[0] ?? 0L);
//        public long? IdPai => (long?)_values[1];
//        public long IdOp => (long)(_values[3] ?? 0L);
//        public long FileId => (long)(_values[2] ?? 0L);
//        public long IdTenant => (long)(_values[4] ?? 0L);
//        public long IdEmpresa => (long)(_values[5] ?? 0L);
//        public string Competencia => (string)(_values[6] ?? string.Empty);
//    }
//}


using System.Buffers;

//public sealed class ValueBuffer : IDisposable
//{
//    private string?[] _fields;

//    public long Id;
//    public long? IdPai;
//    public long FileId;
//    public long IdOp;
//    //public long IdTenant;
//    //public long IdEmpresa;
//    //public string Competencia;

//    public int FieldCount { get; }

//    public const int COLUNAS_FIXAS = 4;
//    public ValueBuffer(int dynamicFieldCount)
//    {
//        //Id = 0;
//        //IdPai = null;
//        //FileId = 0;
//        ////IdOp = 0;
//        ////IdTenant = 0;
//        ////IdEmpresa = 0;
//        //Competencia = string.Empty;

//        FieldCount = dynamicFieldCount;

//        _fields = ArrayPool<string?>.Shared.Rent(dynamicFieldCount);
//    }

//    public void SetField(int index, string? value)
//    {
//        _fields[index] = value;
//    }

//    public string? GetField(int index) => _fields[index];

//    public string?[] Fields => _fields;

//    public void Dispose()
//    {
//        ArrayPool<string?>.Shared.Return(_fields, clearArray: true);
//    }
//}


public struct ValueBuffer
{
    private object?[] _values;
    private bool _fromPool;

    public int FieldCount { get; private set; }

    public const int COLUNAS_FIXAS = 4;

    // ── Etapa 6: fábrica com ArrayPool ───────────────────────────
    /// <summary>
    /// Cria um ValueBuffer alugando array do pool.
    /// Mais performático que new — reutiliza arrays já alocados.
    /// DEVE chamar Return() após uso.
    /// </summary>
    public static ValueBuffer Rent(int fieldCount)
    {
        // ArrayPool.Rent pode retornar array MAIOR que o solicitado
        // por isso guardamos FieldCount separado
        var array = ArrayPool<object?>.Shared.Rent(fieldCount);

        // Limpa manualmente — ArrayPool não garante array zerado
        Array.Clear(array, 0, fieldCount);

        return new ValueBuffer
        {
            _values = array,
            _fromPool = true,
            FieldCount = fieldCount
        };
    }

    /// <summary>
    /// Cria um ValueBuffer sem pool — para testes e casos simples.
    /// </summary>
    public ValueBuffer(int fieldCount)
    {
        FieldCount = fieldCount;
        _values = new object?[fieldCount];
        _fromPool = false;
    }

    // ── Etapa 6: devolve o array ao pool ─────────────────────────
    /// <summary>
    /// Deve ser chamado após o flush do batch para liberar o array.
    /// O SqlBulkWriter chama isso em lista.Clear() → ver FlushAsync.
    /// </summary>
    public void Return()
    {
        if (_fromPool && _values is not null)
        {
            // clearArray: true → limpa antes de devolver (segurança)
            ArrayPool<object?>.Shared.Return(_values, clearArray: true);
            _values = null!;
            _fromPool = false;
        }
    }

    public void Set(int index, object? value)
    {
        _values[index] = value;
    }

    public object? Get(int index) => _values[index];

    /// <summary>
    /// Exposição do array para DataTable.Rows.Add().
    /// Não modificar externamente.
    /// </summary>
    public object?[] Values => _values;

    // Atalhos tipados para campos fixos
    public long Id => (long)(_values[0] ?? 0L);
    public long? IdPai => (long?)_values[1];
    public long FileId => (long)(_values[2] ?? 0L);
    public long IdOp => (long)(_values[3] ?? 0L);
    //public long IdTenant => (long)(_values[4] ?? 0L);
    //public long IdEmpresa => (long)(_values[5] ?? 0L);
    //public string Competencia => (string)(_values[6] ?? string.Empty);
}