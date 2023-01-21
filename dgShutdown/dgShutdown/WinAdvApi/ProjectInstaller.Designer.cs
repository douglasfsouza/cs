namespace WinAdvApi
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.advApiserviceProcessInstall = new System.ServiceProcess.ServiceProcessInstaller();
            this.advApiInstall = new System.ServiceProcess.ServiceInstaller();
            // 
            // advApiserviceProcessInstall
            // 
            this.advApiserviceProcessInstall.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.advApiserviceProcessInstall.Password = null;
            this.advApiserviceProcessInstall.Username = null;
            // 
            // advApiInstall
            // 
            this.advApiInstall.Description = "Advanced Application  Service";
            this.advApiInstall.DisplayName = "AdvApi";
            this.advApiInstall.ServiceName = "AdvApiService";
            this.advApiInstall.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.advApiserviceProcessInstall,
            this.advApiInstall});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller advApiserviceProcessInstall;
        private System.ServiceProcess.ServiceInstaller advApiInstall;
    }
}