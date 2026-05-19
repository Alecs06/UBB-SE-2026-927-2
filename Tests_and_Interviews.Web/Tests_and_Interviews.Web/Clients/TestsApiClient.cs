namespace Tests_and_Interviews.Web.Clients
{
    using Tests_and_Interviews.Web.Dtos;
    public class TestsApiClient
    {
        private readonly HttpClient _http;

        public TestsApiClient(HttpClient http)
        {
            this._http = http;
        }

        public async Task<List<TestDto>> GetAll()
        {
            return await this._http.GetFromJsonAsync<List<TestDto>>("api/tests");
        }

        public async Task<List<string>> GetCategories()
        {
            return await this._http.GetFromJsonAsync<List<string>>($"api/tests/categories");
        }

        public async Task<List<TestDto>> GetByCategory(string category)
        {
            return await this._http.GetFromJsonAsync<List<TestDto>>($"api/tests/category/{category}");
        }

        public async Task<TestDto?> GetById(int id)
        {
            return await this._http.GetFromJsonAsync<TestDto>($"api/tests/{id}");
        }

        public async Task<TestDto?> Create(TestDto dto)
        {
            var response = await this._http.PostAsJsonAsync("api/tests", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TestDto>();
        }

        public async Task Update(int id, TestDto dto)
        {
            var response = await this._http.PutAsJsonAsync($"api/tests/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task Delete(int id)
        {
            var response = await this._http.DeleteAsync($"api/tests/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
