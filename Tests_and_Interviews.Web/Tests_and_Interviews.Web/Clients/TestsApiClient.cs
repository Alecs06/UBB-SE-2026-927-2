namespace Tests_and_Interviews.Web.Clients
{
    using Tests_and_Interviews.Web.Dtos;
    public class TestsApiClient
    {
        private readonly HttpClient _http;
        private static string s_apiPath = "api/tests";

        public TestsApiClient(HttpClient http)
        {
            this._http = http;
        }

        public async Task<List<TestDto>> GetAll()
        {
            return await this._http.GetFromJsonAsync<List<TestDto>>(s_apiPath);
        }

        public async Task<List<string>> GetCategories()
        {
            return await this._http.GetFromJsonAsync<List<string>>($"{s_apiPath}/categories");
        }

        public async Task<List<TestDto>> GetByCategory(string category)
        {
            return await this._http.GetFromJsonAsync<List<TestDto>>($"{s_apiPath}/bycategory/{category}");
        }

        public async Task<TestDto?> GetById(int id)
        {
            return await this._http.GetFromJsonAsync<TestDto>($"{s_apiPath}/{id}");
        }

        public async Task<TestDto?> Create(TestDto dto)
        {
            var response = await this._http.PostAsJsonAsync(s_apiPath, dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TestDto>();
        }

        public async Task Update(int id, TestDto dto)
        {
            var response = await this._http.PutAsJsonAsync($"{s_apiPath}/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task Delete(int id)
        {
            var response = await this._http.DeleteAsync($"{s_apiPath}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}