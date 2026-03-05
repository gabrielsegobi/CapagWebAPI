//using System.Data;

//namespace Infrastructure.Repositories.Bulk
//{
//    public sealed class ValueBufferDataReader : IDataReader
//    {
//        private readonly List<ValueBuffer> _buffers;
//        private readonly string[] _dynamicColumns;
//        private int _index = -1;

//        private static readonly string[] FixedColumns =
//           {
//        "id",
//        "id_pai",
//        "file_id",
//        "id_op"
//    };


//        public ValueBufferDataReader(List<ValueBuffer> buffers, string[] dynamicColumns)
//        {
//            _buffers = buffers ?? throw new ArgumentNullException(nameof(buffers));
//            _dynamicColumns = dynamicColumns;
//        }

//        //public bool Read()
//        //{
//        //    _index++;
//        //    return _index < _buffers.Count;
//        //}

//        //public bool Read()
//        //{
//        //    _index++;

//        //    if (_index >= _buffers.Count)
//        //        return false;

//        //    var buffer = _buffers[_index];

//        //    Console.WriteLine("===== LINHA ENVIADA AO MYSQL =====");

//        //    Console.WriteLine($"Id = {buffer.Id}");
//        //    Console.WriteLine($"IdPai = {buffer.IdPai}");
//        //    Console.WriteLine($"FileId = {buffer.FileId}");
//        //    Console.WriteLine($"IdOp = {buffer.IdOp}");
//        //    //Console.WriteLine($"Competencia = {buffer.Competencia}");

//        //    Console.WriteLine("---- Campos Dinâmicos ----");

//        //    for (int i = 0; i < buffer.FieldCount; i++)
//        //    {
//        //        Console.WriteLine($"Field[{i}] = {buffer.GetField(i)}");
//        //    }

//        //    Console.WriteLine("===================================");

//        //    return true;
//        //}
//        public bool Read() => ++_index < _buffers.Count;

//        //public int FieldCount
//        //    => _buffers.Count == 0 ? 0 : _buffers[0].FieldCount;
//        public int FieldCount => FixedColumns.Length + _dynamicColumns.Length;

//        //public object GetValue(int i)
//        //{
//        //    var value = _buffers[_index].GetField(i);
//        //    return (object?)value ?? DBNull.Value;
//        //}

//        //public object GetValue(int i)
//        //{
//        //    var buffer = _buffers[_index];

//        //    // ── FIXOS ─────────────────────────────
//        //    if (i == 0) return buffer.Id;
//        //    if (i == 1) return (object?)buffer.IdPai ?? DBNull.Value;
//        //    if (i == 2) return buffer.FileId;
//        //    if (i == 3) return buffer.IdOp;

//        //    // ── DINÂMICOS ─────────────────────────
//        //    var columnName = _dynamicColumns[i - FixedColumns.Length];

//        //    if (buffer.TryGetValue(columnName, out var value))
//        //        return value ?? DBNull.Value;

//        //    return DBNull.Value;
//        //}

//        public bool IsDBNull(int i)
//            => _buffers[_index].GetField(i) is null;

//        public Type GetFieldType(int i)
//        {
//            var value = _buffers[0].GetField(i);
//            return value?.GetType() ?? typeof(string);
//        }

//        //public string GetName(int i) => $"Col{i}";

//        public string GetName(int i)
//        {
//            if (i < FixedColumns.Length)
//                return FixedColumns[i];

//            return _dynamicColumns[i - FixedColumns.Length];
//        }
//        public int GetOrdinal(string name)
//        {
//            for (int i = 0; i < FieldCount; i++)
//                if (GetName(i) == name)
//                    return i;

//            throw new IndexOutOfRangeException(name);
//        }

//        public void Dispose() { }

//        #region Membros obrigatórios mínimos

//        public int Depth => 0;
//        public bool IsClosed => false;
//        public int RecordsAffected => -1;
//        public void Close() { }
//        public DataTable GetSchemaTable() => throw new NotImplementedException();
//        public bool NextResult() => false;

//        public int GetValues(object[] values)
//        {
//            var buffer = _buffers[_index];

//            int count = Math.Min(values.Length, buffer.FieldCount);

//            for (int i = 0; i < count; i++)
//                values[i] = (object?)buffer.GetField(i) ?? DBNull.Value;

//            return count;
//        }

//        public object GetValue(int i)
//        {
//            var buffer = _buffers[_index];

//            // FIXOS
//            if (i == 0) return buffer.Id;
//            if (i == 1) return (object?)buffer.IdPai ?? DBNull.Value;
//            if (i == 2) return buffer.FileId;
//            if (i == 3) return buffer.IdOp;

//            // DINÂMICOS
//            var dynamicIndex = i - ValueBuffer.COLUNAS_FIXAS;
//            return (object?)buffer.GetField(dynamicIndex) ?? DBNull.Value;
//        }



//        // Tipos fortemente tipados delegam para GetValue
//        public string GetString(int i) => (string)GetValue(i);
//        public int GetInt32(int i) => Convert.ToInt32(GetValue(i));
//        public long GetInt64(int i) => Convert.ToInt64(GetValue(i));
//        public short GetInt16(int i) => Convert.ToInt16(GetValue(i));
//        public bool GetBoolean(int i) => Convert.ToBoolean(GetValue(i));
//        public DateTime GetDateTime(int i) => Convert.ToDateTime(GetValue(i));
//        public decimal GetDecimal(int i) => Convert.ToDecimal(GetValue(i));
//        public double GetDouble(int i) => Convert.ToDouble(GetValue(i));
//        public float GetFloat(int i) => Convert.ToSingle(GetValue(i));
//        public Guid GetGuid(int i) => (Guid)GetValue(i);

//        public byte GetByte(int i) => Convert.ToByte(GetValue(i));
//        public char GetChar(int i) => Convert.ToChar(GetValue(i));

//        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
//            => throw new NotImplementedException();

//        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
//            => throw new NotImplementedException();

//        public IDataReader GetData(int i)
//            => throw new NotImplementedException();

//        public string GetDataTypeName(int i)
//            => GetFieldType(i).Name;

//        public object this[int i] => GetValue(i);
//        public object this[string name] => GetValue(GetOrdinal(name));

//        #endregion
//    }
//}