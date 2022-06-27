using ActiveXTService.Data;
using ActiveXTService.Data.Model;
using ActiveXTService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;


namespace ActiveXTService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<SftpSettingsModel> _appSettings;
        private readonly IServiceProvider _provider;
        //1st File set time Too old
        private DateTime OldestFileDt; 

        public Worker(ILogger<Worker> logger, IOptions<SftpSettingsModel> app, IServiceProvider provider)
        {
            _logger = logger;
            _appSettings = app;
            _provider = provider;
            init();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Statr", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                string fileName = "";
                _logger.LogInformation("1::--Service Resume", DateTimeOffset.Now);
                try
                {
                    OldestFileDt = FileMaxTime();
                    _logger.LogInformation("2:: Get File Max Time", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError("1.1::--MaxTime Database Error"+ex.Message, DateTimeOffset.Now);
                    continue;

                }
                using (var sftp = new SftpClient(_appSettings.Value.Server, Convert.ToInt16(_appSettings.Value.Port), _appSettings.Value.UserName, _appSettings.Value.Password))
                {
                    try
                    {
                        sftp.Connect();
                        _logger.LogInformation("3::--SFTP connection stablished", DateTimeOffset.Now);

                        //Serch new file on sftp server
                        var remotefiles = sftp.ListDirectory("/").Where(f => f.LastAccessTime > OldestFileDt);
                        _logger.LogInformation("4::--SFTP File found on server::--", DateTimeOffset.Now);
                        int counter = 1;
                        foreach (SftpFile RemoteFileName in remotefiles)
                        {
                            //File write on local Disc
                            try
                            {
                                using (var file = File.OpenWrite(_appSettings.Value.FileLocalSavePath + "\\" + RemoteFileName.Name))
                                {
                                    sftp.DownloadFile(RemoteFileName.Name, file);
                                    fileName = _appSettings.Value.FileLocalSavePath + "\\" + RemoteFileName.Name;
                                    _logger.LogInformation("5::--SFTP File download and write to disc--Success . File No::--" + counter, DateTimeOffset.Now);

                                }
                                //File and Log write to DB
                                try
                                {
                                    //File write to DB
                                    FileWriteToDB(RemoteFileName.Name, RemoteFileName.LastWriteTime, File.ReadAllBytes(fileName), fileName, "Success");
                                    _logger.LogInformation("6::--SFTP File write to DB--Success . File No::--" + counter, DateTimeOffset.Now);
                                    //Log on DB
                                    LogWriteToDB("Info", "Test");
                                    _logger.LogInformation("7::--Log write to DB--Success . File No::--" + counter++, DateTimeOffset.Now);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("7.1::--SFTP file db write fail. Message-- " + ex.Message, DateTimeOffset.Now);
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("5.1::--SFTP File download and write fail. Message::" + ex.Message, DateTimeOffset.Now);
                            }
                            _logger.LogInformation("6::--SFTP File Write to disk and DB done.", DateTimeOffset.Now);
                        }
                        _logger.LogInformation("7::--SFTP Closse::--", DateTimeOffset.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("SFTP server connect fail::" + ex.Message, DateTimeOffset.Now);
                        continue;
                    }
                }
                _logger.LogInformation("9::--Service sleep for Next " + Convert.ToInt16(_appSettings.Value.Interval) / 1000 + " Seconds", DateTimeOffset.Now);
                await Task.Delay(Convert.ToInt16(_appSettings.Value.Interval), stoppingToken);
            }
        }
        /*
         * Log Write To DB
         * DB Tbl Name ::Log
         * 
         */
        private void LogWriteToDB(string LogType,string LogData)
        {
            try
            {
                var log = new Log() { LogType = LogType, LogData = LogData };
                using (var scope = _provider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SftpDataContext>();
                    dbContext.Entry(log).State = EntityState.Added;
                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        /*
         * Fil eWrite To DB
         * DB Tbl Name ::ImportedFile
         */
        private void FileWriteToDB(string FileName, DateTime CreatedDate, byte[] FileContent, string fileName, string status)
        {
            try
            {
                var importedFile = new ImportedFile()
                {
                    FileName = FileName,
                    CreatedDate = CreatedDate,
                    FileContent = FileContent,
                    FilePath = fileName,
                    Status = status
                };
                using (var scope = _provider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SftpDataContext>();
                    dbContext.Entry(importedFile).State = EntityState.Added;
                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        /*
         * Initialize SFTP local Directory
         */ 
        private void init()
        {
            //create directory if not exist
            if (!Directory.Exists(_appSettings.Value.FileLocalSavePath))
                Directory.CreateDirectory(_appSettings.Value.FileLocalSavePath);
        }
        /*
         * Get database Max(file save time) 
         * To compare with new file
         */
        private DateTime FileMaxTime()
        {
            DateTime dt=new DateTime();
            try
            {
                using (var scope = _provider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SftpDataContext>();
                    dt = dbContext.ImportedFiles.OrderByDescending(c => c.CreatedDate)
                                                           .Select(c => c.CreatedDate)
                                                           .FirstOrDefault();
                    //Increase last file with few seconds
                    //dt=dt.AddSeconds(Convert.ToInt16(_appSettings.Value.Interval) / 1000 / 2);
                }
            }
            catch 
            {
                throw;
            }
            return dt;
        }
    }
}