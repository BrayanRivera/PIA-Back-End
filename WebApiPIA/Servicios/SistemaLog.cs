namespace WebApiPIA.Servicios
{
    public class SistemaLog : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string sistemaLog = "sistemaLog.txt";
        private Timer timer;

        public SistemaLog(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            Write("Sistema iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("Sistema apagado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Write(string msj)
        {
            var ruta = $@"{env.ContentRootPath}/wwwroot/{sistemaLog}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) 
            {
                writer.WriteLine(msj);
            }
        } 
    }
}
