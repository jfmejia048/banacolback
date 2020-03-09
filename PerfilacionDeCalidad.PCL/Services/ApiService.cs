using Newtonsoft.Json;
using PerfilacionDeCalidad.PCL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.PCL.Services
{
    public class ApiService
    {
        public async Task<Response> Post(string controller, object model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.75:84/api/");
                var response = await client.PostAsync(controller, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        success = false,
                        data = null
                    };
                }
                var obj = JsonConvert.DeserializeObject<Response>(answer);
                return new Response
                {
                    success = true,
                    data = obj.data
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    success = false,
                    data = ex.Message
                };
            }
        }

        public async Task<Response> Post(string controller, object model, string accessToken)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.75:84/api/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(controller, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        success = false,
                        data = null
                    };
                }
                var obj = JsonConvert.DeserializeObject<Response>(answer);
                return new Response
                {
                    success = true,
                    data = obj.data
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    success = false,
                    data = ex.Message
                };
            }
        }
    }
}
