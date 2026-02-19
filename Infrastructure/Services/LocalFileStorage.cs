using Domain.Interfaces;


namespace Infrastructure.Services
{
    public class LocalFileStorage : IObjectStorage
    {
        private readonly string _basePath;

        public LocalFileStorage(string basePath)
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath);
        }

        //public async Task SaveAsync(string key, Stream data)
        //{
        //    var path = Path.Combine(_basePath, key);
        //    Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        //    using var file = File.Create(path);
        //    await data.CopyToAsync(file);
        //}

        public async Task SaveAsync(string folderKey, Stream data, string fileName)
        {
            // key = nome da pasta
            var folderPath = Path.Combine(_basePath, folderKey);

            // cria a pasta se não existir
            Directory.CreateDirectory(folderPath);

            // garante nome único
            //var uniqueName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using var file = File.Create(filePath);
            await data.CopyToAsync(file);
        }

        //public async Task SaveAsync(string key, Stream data)
        //{
        //    // key esperada: cnpjRaiz-competencia
        //    var partes = key.Split('-', StringSplitOptions.RemoveEmptyEntries);

        //    if (partes.Length != 2)
        //        throw new ArgumentException("Key inválida. Use o formato cnpjRaiz-competencia.");

        //    var cnpjRaiz = partes[0];
        //    var competencia = partes[1];

        //    var directoryPath = Path.Combine(_basePath, cnpjRaiz, competencia);
        //    Directory.CreateDirectory(directoryPath);

        //    var filePath = Path.Combine(directoryPath, "arquivo.ecf");

        //    using var file = File.Create(filePath);
        //    await data.CopyToAsync(file);
        //}


        //public Task<Stream> OpenReadAsync(string key)
        //{
        //    // key esperada: cnpjRaiz-competencia
        //    var partes = key.Split('-', StringSplitOptions.RemoveEmptyEntries);

        //    if (partes.Length != 2)
        //        throw new ArgumentException("Key inválida. Use o formato cnpjRaiz-competencia.");

        //    var cnpjRaiz = partes[0];
        //    var competencia = partes[1];

        //    var directoryPath = Path.Combine(_basePath, cnpjRaiz, competencia);

        //    if (!Directory.Exists(directoryPath))
        //        throw new DirectoryNotFoundException("Diretório não encontrado para a key informada.");

        //    var filePath = Directory
        //        .EnumerateFiles(directoryPath)
        //        .FirstOrDefault();

        //    if (filePath == null)
        //        throw new FileNotFoundException("Nenhum arquivo encontrado para a key informada.");

        //    Stream stream = File.OpenRead(filePath);
        //    return Task.FromResult(stream);
        //}

        public Task<Stream> OpenReadAsync(string directoryPath, string fileName)
        {

            var fullPath = Path.Combine(_basePath, directoryPath);

            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Diretório inválido.", nameof(fullPath));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Nome do arquivo inválido.", nameof(fileName));

            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException($"Diretório não encontrado: {fullPath}");

            var filePath = Path.Combine(fullPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado.", filePath);

            Stream stream = File.Open(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            return Task.FromResult(stream);
        }


        //public Task<Stream> OpenReadAsync(string key)
        //{
        //    var path = Path.Combine(_basePath, key);
        //    Stream stream = File.OpenRead(path);
        //    return Task.FromResult(stream);
        //}

        public Task DeleteAsync(string key)
        {
            File.Delete(Path.Combine(_basePath, key));
            return Task.CompletedTask;
        }

        public bool Exists(string key)
            => File.Exists(Path.Combine(_basePath, key));
    }

}
