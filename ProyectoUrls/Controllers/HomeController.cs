using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProyectoUrls.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProyectoUrls.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration Configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            Configuration = _configuration;
        }
        [HttpGet("{code?}")]
        public async Task<IActionResult> Index(string code)
        {
            var model = new TinyUrl();
            //evaluamos que venga un parametro en la URL
            if (!String.IsNullOrEmpty(code))
            {
                //obtenemos la conexion desdea el archivo appsetting.json
                string connString = this.Configuration.GetConnectionString("conexion");
                //instanciamos la conexion
                SqlConnection conexion = new SqlConnection(connString);
                try
                {
                    //abrimos la conexion a la base de datos
                    conexion.Open();
                    //creamos la query
                    string sqlQuery = "select* from redirect.TinyUrl where Codigo=@codigo";
                    // creamos el comando para la base de datos
                    SqlCommand comando = new SqlCommand(sqlQuery, conexion);
                    //parametrizamos la consulta para evitar inyeccion SQL
                    comando.Parameters.Add("@codigo", System.Data.SqlDbType.VarChar);
                    comando.Parameters["@codigo"].Value = code;
                    SqlDataReader registro = comando.ExecuteReader();
                    //si al ejecutar la sentencia se obtuvo al menos un registro
                    if (registro.Read())
                    {
                        //se crea un ubjeto de la tabla para luego evaluar las propiedades
                        model.Codigo = registro["Codigo"].ToString();
                        model.DestUrl = registro["DestUrl"].ToString();
                        model.IsEnabled = bool.Parse(registro["IsEnabled"].ToString());

                    }
                    else
                    {
                        model.Mensaje = "Aviso!No se encontró ningun registro con el parametro indicado";
                    }
                }
                finally
                {
                    conexion.Close();
                }
            }
            else
            {
                model.Mensaje = "No se encontraron parametros en la URL. Ej. https://misitio.com?page=0002";
            }
            //si el registro existe y está activo en la base de datos
            if (model.Codigo != null && model.IsEnabled)
            {
                return Redirect(model.DestUrl);

            }//si el registro no está activo en la base de datos
            else if (model.Codigo != null && !model.IsEnabled)
            {
                model.Mensaje = "El parametro brindado no está activo";
            }
            //en caso de que haya que redirigir a la vista donde se muestra la imagen se envia el model el cual lleva un mensaje a mostrar
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
