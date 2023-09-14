using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedCrossChat.Domain
{
    public class SeedHelper
    {
        public static async Task<List<TEntity>> GetSeedData<TEntity>(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = "wwwroot/JsonFiles";
            string fullPath = Path.Combine(currentDirectory, path, fileName);

            var data = await File.ReadAllTextAsync(fullPath);
            //var results = JsonSerializer.Deserialize<List<TEntity>>(elisaPlateDefsData);
            return JsonSerializer.Deserialize<List<TEntity>>(data);
        }
    }
}
