using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Pruebas_selenium
{
    public class Tests
    {
        private IWebDriver driver;
        private string rutaDelProyecto;
        private string carpetaFotos; 

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
            string rutaRelativa = Path.Combine(directorioBase, @"..\..\..\..\Pagina web");
            rutaDelProyecto = Path.GetFullPath(rutaRelativa);

            string rutaEvidencias = Path.Combine(directorioBase, @"..\..\..\..\Evidencias");
            carpetaFotos = Path.GetFullPath(rutaEvidencias);

            if (!Directory.Exists(carpetaFotos))
            {
                Directory.CreateDirectory(carpetaFotos);
            }
        }

        [Test]
        public void T01_Login_Exitoso()
        {
            Navegar("index.html");

            driver.FindElement(By.Id("username")).SendKeys("admin");
            driver.FindElement(By.Id("password")).SendKeys("1234");
            driver.FindElement(By.Id("login-btn")).Click();
            Thread.Sleep(1000);

            bool loginExitoso = driver.Url.Contains("dashboard.html") || driver.FindElements(By.Id("logout-btn")).Count > 0;
            Assert.That(loginExitoso, Is.True, "El login falló.");
        }

        [Test]
        public void T02_Login_Fallido()
        {
            Navegar("index.html");

            driver.FindElement(By.Id("username")).SendKeys("admin");
            driver.FindElement(By.Id("password")).SendKeys("error");
            driver.FindElement(By.Id("login-btn")).Click();

            var errorMsg = driver.FindElement(By.Id("error-msg"));
            Assert.That(errorMsg.Displayed, Is.True, "No apareció mensaje de error.");
        }

        [Test]
        public void T03_CRUD_Crear()
        {
            HacerLoginRapido();

            driver.FindElement(By.Id("product-name")).SendKeys("Monitor 4K");
            driver.FindElement(By.Id("product-price")).SendKeys("300");
            driver.FindElement(By.Id("submit-btn")).Click();

            var tabla = driver.FindElement(By.Id("table-body"));
            Assert.That(tabla.Text, Does.Contain("Monitor 4K"));
        }

        [Test]
        public void T04_CRUD_Editar()
        {
            HacerLoginRapido();
            CrearProductoDummy("Silla Gamer", "100");

            driver.FindElement(By.CssSelector(".edit")).Click();

            var inputNombre = driver.FindElement(By.Id("product-name"));
            inputNombre.Clear();
            inputNombre.SendKeys("Silla Oficina");

            driver.FindElement(By.Id("submit-btn")).Click();

            var tabla = driver.FindElement(By.Id("table-body"));
            Assert.That(tabla.Text, Does.Contain("Silla Oficina"));
        }

        [Test]
        public void T05_CRUD_Eliminar()
        {
            HacerLoginRapido();
            CrearProductoDummy("Teclado Mecanico", "50");

            driver.FindElement(By.CssSelector(".delete")).Click();
            driver.SwitchTo().Alert().Accept();
            Thread.Sleep(500); 

            var tabla = driver.FindElement(By.Id("table-body"));
            Assert.That(tabla.Text.Contains("Teclado Mecanico"), Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                try
                {
                    TomarCapturaPantalla();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo tomar foto: " + ex.Message);
                }
                finally
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
        }

        private void TomarCapturaPantalla()
        {
            ITakesScreenshot camara = driver as ITakesScreenshot;
            Screenshot foto = camara.GetScreenshot();

            string nombrePrueba = TestContext.CurrentContext.Test.Name;
            string nombreArchivo = Path.Combine(carpetaFotos, nombrePrueba + ".png");

            foto.SaveAsFile(nombreArchivo);
            TestContext.AddTestAttachment(nombreArchivo); 
        }

        private void Navegar(string archivo)
        {
            string url = Path.Combine(rutaDelProyecto, archivo);
            driver.Navigate().GoToUrl(url);
        }

        private void HacerLoginRapido()
        {
            Navegar("index.html");
            driver.FindElement(By.Id("username")).SendKeys("admin");
            driver.FindElement(By.Id("password")).SendKeys("1234");
            driver.FindElement(By.Id("login-btn")).Click();
        }

        private void CrearProductoDummy(string nombre, string precio)
        {
            driver.FindElement(By.Id("product-name")).SendKeys(nombre);
            driver.FindElement(By.Id("product-price")).SendKeys(precio);
            driver.FindElement(By.Id("submit-btn")).Click();
        }
    }
}