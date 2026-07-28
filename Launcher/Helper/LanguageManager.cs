using System.Collections.Generic;
using System.IO;

namespace Nelian
{
    public static class LanguageManager
    {
        private static string _currentLanguage = "en";
        public static string CurrentLanguage => _currentLanguage;

        private static string LanguageFilePath => Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "Game",
            "Nelian",
            "index.language"
        );

        static LanguageManager()
        {
            LoadLanguage();
        }

        private static void LoadLanguage()
        {
            try
            {
                if (File.Exists(LanguageFilePath))
                {
                    string lang = File.ReadAllText(LanguageFilePath).Trim().ToLower();
                    if (lang == "tr" || lang == "en")
                    {
                        _currentLanguage = lang;
                        return;
                    }
                }
            }
            catch { }
            _currentLanguage = "en";
        }

        public static void SetLanguage(string lang)
        {
            if (lang != "tr" && lang != "en")
                lang = "en";

            _currentLanguage = lang;

            try
            {
                string dir = Path.GetDirectoryName(LanguageFilePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(LanguageFilePath, lang);
            }
            catch { }
        }

        private static readonly Dictionary<string, string> TR = new()
        {
            // Splash
            ["Splash.CheckingUpdates"] = "Güncellemeler kontrol ediliyor...",
            ["Splash.InstallingUpdates"] = "Güncellemeler yükleniyor...",
            ["Splash.PleaseWait"] = "Lütfen bekleyin...",
            ["Splash.AllSet"] = "Her şey hazır!",
            ["Splash.FolderCreateError"] = "Klasör oluşturma hatası: {0}",
            ["Splash.ScalingWarning"] = "Windows ekran ölçeklendirmeniz %{0} olarak ayarlanmış.\n\nEn iyi deneyim için lütfen %100 olarak ayarlayın.\n\nNelian'ı yeniden başlatmak ister misiniz?",
            ["Splash.ScalingTitle"] = "Nelian Uyarısı",
            ["Splash.Error"] = "Hata",
            ["Splash.DownloadErrorMessage"] = "İndirme hatası: {0}\n\nLütfen internet bağlantınızı kontrol edip tekrar deneyin.",
            ["Splash.UpdateCheckFailed"] = "Güncelleme kontrolü başarısız: {0}. Tekrar deneniyor...",
            ["Splash.DownloadFileFailed"] = "İndirme başarısız: {0} - {1}",
            ["Splash.ProcessError"] = "İşlem hatası ({0}): {1}",
            ["Splash.ExtractFailed"] = "Çıkarma hatası: {0}",
            ["Splash.HashMismatch"] = "Bir şeyler yanlış gitti!",
            ["Splash.HashVerificationFailed"] = "Bizim tarafımızda bir sorun oluştu, en kısa sürede düzeltilecektir.",
            ["Splash.HashError"] = "Hash doğrulama hatası: {0}",
            ["Splash.CheckingData"] = "Veriler kontrol ediliyor...",
            ["Splash.Downloading"] = "İndiriliyor...",
            ["Splash.DownloadError"] = "İndirme hatası: {0}",
            ["Splash.Launching"] = "Başlatılıyor...",
            ["Splash.CheckingFiles"] = "Dosyalar kontrol ediliyor...",
            ["Splash.Starting"] = "Başlatılıyor...",
            ["Splash.Extracting"] = "Çıkarılıyor...",
            ["Splash.UpdaterNotFound"] = "Güncelleme Dosyaları Bulunamadı, Lutfen tekrar Kurun",

            // Main
            ["Main.Accounts"] = "Hesaplar",
            ["Main.Launch"] = "Oyunu Başlat",
            ["Main.NoSession"] = "Geçerli oturum bulunamadı!",
            ["Main.ErrorTitle"] = "Nelian Hatası",
            ["Main.RuntimeNotFound"] = "Nelian Kütüphane dosyaları bulunamadı! Devam etmek için lütfen başlatıcıyı yeniden başlatın.",
            ["Main.CorruptedFile"] = "Bu Nelian sürümü resmi olmayabilir!\n\nGüvenlik nedeniyle başlatma sonlandırıldı.\nDosya bozulmuş veya değiştirilmiş.\nLütfen doğru sürümü indirmek için başlatıcıyı yeniden başlatın.\n\nHata: 2043",
            ["Main.PrepareFailed"] = "Nelian hazırlanırken hata oluştu: {0}",
            ["Main.NetworkError"] = "Nelian Ağına bağlanılamıyor: {0}",
            ["Main.CrashMessage"] = "Minecraft beklenmedik şekilde kapandı.\n\nSebep:\n{0}\n\nÇıkış Kodu: {1}",
            ["Main.SettingsSaveError"] = "Ayarlar kaydedilemedi. Lütfen klasör izinlerini kontrol edin.",
            ["Main.SettingsSaveErrorDetail"] = "Ayarlar kaydedilirken hata: {0}",
            ["Main.CrashDetected"] = "Çökme Algılandı",
            ["Main.PlayTime"] = "Oynama Süresi: {0}",
            ["Main.LaunchingVanilla"] = "{0} başlatılıyor...",
            ["Main.ReadyVanilla"] = "{0} başlatılmaya hazır",
            ["Main.ReadyNelian"] = "Nelian başlatılmaya hazır",
            ["Main.VerifyingAssets"] = "Varlıklar doğrulanıyor..",
            ["Main.PreparingFiles"] = "Dosyalar hazırlanıyor...",
            ["Main.VerifyingFiles"] = "Dosyalar doğrulanıyor...",
            ["Main.UpdateRequired"] = "Güncelleme gerekli!",
            ["Main.Launching"] = "Başlatılıyor...",
            ["Main.ReadyVersion"] = "Başlatmaya hazır",
            ["Main.VersionTypeTitle"] = "Sürüm",
            ["Main.VersionTypeDesc"] = "Başlatılacak sürümü seçin",
            ["Main.SelectVersion"] = "Sürüm Seçin:",
            ["Main.BanText"] = "Kısıtlandınız..",

            // Axion
            ["Axion.Security"] = "Güvenlik",
            ["Axion.ActivityDetected"] = "Güvenlik ihlali tespit edildi, giriş engellendi.",
            ["Axion.ActivityDesc"] = "Cihazınızda şüpheli aktivite tespit edildi, lütfen Başlatıcınızı tekrar başlatın.",
            ["Axion.VerifyIdentity"] = "Kimliği Doğrula",
            ["Axion.IdentityVerified"] = "Kimlik Doğrulandı",

            // Settings Panel
            ["Main.SettingsTitle"] = "Ayarlar",
            ["Main.MemoryAllocation"] = "Bellek Tahsisi",
            ["Main.MemoryDesc"] = "Minecraft için ayrılacak RAM miktarını belirleyin",
            ["Main.MemoryRecommend"] = "Önerilen: 4096 MB (4 GB)",
            ["Main.VanillaTitle"] = "Vanilla Sürümleri Başlat",
            ["Main.VanillaDesc"] = "Vanilla veya Optifine sürümlerini başlatmayı etkinleştir.",
            ["Main.AnimTitle"] = "Canlı Animasyonlar",
            ["Main.AnimDesc"] = "Akıcı animasyonları etkinleştir",
            ["Main.RpcTitle"] = "Discord RPC",
            ["Main.RpcDesc"] = "Discord Rich Presence'i etkinleştir",
            ["Main.FullscreenTitle"] = "Tam Ekran Modu",
            ["Main.FullscreenDesc"] = "Minecraft'ı tam ekran başlat",
            ["Main.ServerTitle"] = "Sunucu IP",
            ["Main.ServerDesc"] = "Başlatmada belirli bir sunucuya bağlan",
            ["Main.ServerAddress"] = "Sunucu Adresi:",
            ["Main.ThemeTitle"] = "Tema",
            ["Main.ThemeDesc"] = "Başlatıcı temasını seçin",
            ["Main.BehaviorTitle"] = "Başlatma Davranışı",
            ["Main.BehaviorDesc"] = "Oyun başladıktan sonra başlatıcı ne yapsın",
            ["Main.BehaviorKeepOpen"] = "Açık Tut",
            ["Main.BehaviorMinimize"] = "Küçült (Varsayılan)",
            ["Main.BehaviorClose"] = "Kapat",
            ["Main.LanguageTitle"] = "Dil",
            ["Main.LanguageDesc"] = "Başlatıcı dilini seçin",
            ["Main.LanguageEnglish"] = "English",
            ["Main.LanguageTurkish"] = "Türkçe",
            ["Main.LanguageRestart"] = "Dil değişikliği için uygulamayı yeniden başlatın.",
            ["Main.TokenError"] = "Token oluşturulamadı!",
            ["Freely.Downloading"] = "FreelyMC jar indiriliyor...",
            ["Freely.DownloadComplete"] = "FreelyMC jar indirildi.",
            ["Freely.DownloadError"] = "FreelyMC jar indirilemedi: {0}",
            ["Main.Banned"] = "Hesabınız {0} tarihine kadar yasaklanmıştır.",
            ["Main.BanReason"] = "Ban Sebebi: {0}",
            ["Main.Bannedperm"] = "Hesabınız kalıcı olarak yasaklanmıştır.",
            ["Main.SuspendedUntil"] = "{0} tarihine kadar yasaklandınız.",
            ["Main.SuspendedPermanent"] = "Kalıcı olarak yasaklandınız.",
            ["Main.WaitAMin"] = "Başlatıcı bütünlüğü kontrol ediliyor..",
            // AccountManager
            ["AccountManager.NoAccounts"] = "Hesap bulunamadı. 'Hesap Ekle' butonuna tıklayarak hesap ekleyin.",
            ["AccountManager.GettingReady"] = "Giriş Yapılıyor...",
            ["AccountManager.PleaseWait"] = "Lütfen Bekleyin",
            ["AccountManager.AddAccount"] = "Hesap Ekle",
            ["AccountManager.LoginFailed"] = "Giriş başarısız: {0}",
            ["AccountManager.RemoveFailed"] = "Silme başarısız: {0}",
            ["AccountManager.RefreshError"] = "Yenileme hatası: {0}",
            ["AccountManager.OfflineTitle"] = "Bir Kullanıcı adı seçiniz!",
            ["AccountManager.OfflineMode"] = "Çevrimdışı",
            ["AccountManager.OfflineSave"] = "Kaydet",
            ["AccountManager.UsernameEmpty"] = "Kullanıcı adı boş olamaz.",
            ["AccountManager.UsernameShort"] = "Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır.",
            ["AccountManager.UsernameSpaces"] = "Kullanıcı adı boşluk içeremez.",
            ["AccountManager.KeyFileError"] = "Anahtar dosyası hatası: {0}",
            ["AccountManager.UsernameInvalid"] = "Kullanıcı Adı geçersiz karakterler içeriyor.",
            ["AccountManager.Microsoft"] = "Microsoft",
            ["AccountManager.SelectAccountType"] = "Lütfen Hesap türünü seçin",
            ["AccountManager.Username"] = "Kullanıcı Adı",
            ["AccountManager.CannotBeOfflineAsDeveloper"] = "Bir geliştiricinin ismini kullanamazsınız!",
            ["AccountManager.UsernameLong"] = "Kullanıcı adı 16 karakterden uzun olamaz.",

            // AccountControl
            ["AccountControl.Login"] = "Giriş Yap",
            ["AccountControl.Remove"] = "Sil",
        };

        private static readonly Dictionary<string, string> EN = new()
        {
            // Splash
            ["Splash.CheckingUpdates"] = "Checking for updates...",
            ["Splash.InstallingUpdates"] = "Installing updates...",
            ["Splash.PleaseWait"] = "Please wait...",
            ["Splash.AllSet"] = "All set!",
            ["Splash.FolderCreateError"] = "Folder creation error: {0}",
            ["Splash.ScalingWarning"] = "Your Windows display scaling is set to {0}%.\n\nFor the best experience, please set it to 100%.\n\nWould you like to restart Nelian to try again?",
            ["Splash.ScalingTitle"] = "Nelian Warning",
            ["Splash.Error"] = "Error",
            ["Splash.DownloadErrorMessage"] = "Download error: {0}\n\nPlease check your internet connection and try again.",
            ["Splash.UpdateCheckFailed"] = "Update check failed: {0}. Trying again...",
            ["Splash.DownloadFileFailed"] = "Download failed: {0} - {1}",
            ["Splash.ProcessError"] = "Processing error ({0}): {1}",
            ["Splash.ExtractFailed"] = "Extraction failed: {0}",
            ["Splash.HashMismatch"] = "Something went wrong!",
            ["Splash.HashVerificationFailed"] = "Something went wrong on our side, will be fixed as soon as possible.",
            ["Splash.HashError"] = "Hash verification error: {0}",
            ["Splash.CheckingData"] = "Checking data...",
            ["Splash.Downloading"] = "Downloading...",
            ["Splash.DownloadError"] = "Download error: {0}",
            ["Splash.Launching"] = "Launching...",
            ["Splash.CheckingFiles"] = "Checking files...",
            ["Splash.Starting"] = "Starting...",
            ["Splash.Extracting"] = "Extracting...",

            // Main
            ["Main.Accounts"] = "Accounts",
            ["Main.Launch"] = "Launch",
            ["Main.NoSession"] = "No current Session Found!",
            ["Main.ErrorTitle"] = "Nelian Error",
            ["Main.RuntimeNotFound"] = "Runtime not found! Please restart your Launcher to continue.",
            ["Main.CorruptedFile"] = "This version of Nelian might not be official!\n\nFor security reasons, your launch has been terminated.\nThe file has been corrupted or modified.\nPlease restart the launcher to download the correct version.\n\nError: 2043",
            ["Main.PrepareFailed"] = "Failed to prepare Nelian: {0}",
            ["Main.NetworkError"] = "Cannot connect to Nelian Network: {0}",
            ["Main.CrashMessage"] = "Minecraft exited unexpectedly.\n\nReason:\n{0}\n\nExit Code: {1}",
            ["Main.SettingsSaveError"] = "Settings could not be saved. Please check folder permissions.",
            ["Main.SettingsSaveErrorDetail"] = "Error saving settings: {0}",
            ["Main.CrashDetected"] = "Crash Detected",
            ["Main.PlayTime"] = "Play Time: {0}",
            ["Main.LaunchingVanilla"] = "Launching {0}...",
            ["Main.ReadyVanilla"] = "Ready to Launch {0}",
            ["Main.ReadyNelian"] = "Ready to Launch Nelian",
            ["Main.VerifyingAssets"] = "Verifying Assets..",
            ["Main.PreparingFiles"] = "Preparing files...",
            ["Main.VerifyingFiles"] = "Verifying files...",
            ["Main.UpdateRequired"] = "Update required!",
            ["Main.Launching"] = "Launching...",
            ["Main.ReadyVersion"] = "Ready to launch",
            ["Main.VersionTypeTitle"] = "Version",
            ["Main.VersionTypeDesc"] = "Select version to launch",
            ["Main.SelectVersion"] = "Select Version:",
            ["Main.BanText"] = "Restricted..",

            // Axion
            ["Axion.Security"] = "Security",
            ["Axion.ActivityDetected"] = "Security violation detected, access denied.",
            ["Axion.ActivityDesc"] = "We have detected suspicious activity on your device. Restart your Launcher to continue.",
            ["Axion.VerifyIdentity"] = "Verify Identity",
            ["Axion.IdentityVerified"] = "Identity Verified",

            // Settings Panel
            ["Main.SettingsTitle"] = "Settings",
            ["Main.MemoryAllocation"] = "Memory Allocation",
            ["Main.MemoryDesc"] = "Set the amount of RAM to allocate for Minecraft",
            ["Main.MemoryRecommend"] = "Recommended: 4096 MB (4GB)",
            ["Main.VanillaTitle"] = "Launch Vanilla Versions",
            ["Main.VanillaDesc"] = "Enable to launch Vanilla or Optifine versions.",
            ["Main.AnimTitle"] = "Live Animations",
            ["Main.AnimDesc"] = "Enable smooth animations",
            ["Main.RpcTitle"] = "Discord RPC",
            ["Main.RpcDesc"] = "Enable Discord Rich Presence",
            ["Main.FullscreenTitle"] = "Fullscreen Mode",
            ["Main.FullscreenDesc"] = "Launch Minecraft in fullscreen",
            ["Main.ServerTitle"] = "Server IP",
            ["Main.ServerDesc"] = "Connect to a specific server on launch",
            ["Main.ServerAddress"] = "Server Address:",
            ["Main.ThemeTitle"] = "Theme",
            ["Main.ThemeDesc"] = "Choose launcher theme",
            ["Main.BehaviorTitle"] = "Launch Behavior",
            ["Main.BehaviorDesc"] = "What to do with launcher after game starts",
            ["Main.BehaviorKeepOpen"] = "Keep Open",
            ["Main.BehaviorMinimize"] = "Minimize (Default)",
            ["Main.BehaviorClose"] = "Close",
            ["Main.LanguageTitle"] = "Language",
            ["Main.LanguageDesc"] = "Select launcher language",
            ["Main.LanguageEnglish"] = "English",
            ["Main.LanguageTurkish"] = "Türkçe",
            ["Main.LanguageRestart"] = "Please restart the application to apply language change.",
            ["Main.TokenError"] = "Token could not be created!",
            ["Freely.Downloading"] = "Downloading FreelyMC jar...",
            ["Freely.DownloadComplete"] = "FreelyMC jar downloaded.",
            ["Freely.DownloadError"] = "FreelyMC jar could not be downloaded: {0}",
            ["Main.Banned"] = "Your Account has been suspended until {0}.",
            ["Main.Bannedperm"] = "Your Account has been permanently suspended.",
            ["Main.BanReason"] = "Ban Reason: {0}",
            ["Main.SuspendedUntil"] = "You've been suspended until {0}.",
            ["Main.SuspendedPermanent"] = "You've been suspended permanently.",
            ["Main.WaitAMin"] = "Fetching Launcher Data..",
            // AccountManager
            ["AccountManager.NoAccounts"] = "No accounts found. Click 'Add Account' to add one.",
            ["AccountManager.GettingReady"] = "Getting things ready...",
            ["AccountManager.PleaseWait"] = "Please Wait",
            ["AccountManager.AddAccount"] = "Add Account",
            ["AccountManager.LoginFailed"] = "Login failed: {0}",
            ["AccountManager.RemoveFailed"] = "Remove failed: {0}",
            ["AccountManager.RefreshError"] = "Refresh error: {0}",
            ["AccountManager.OfflineTitle"] = "Pick any username you like!",
            ["AccountManager.OfflineMode"] = "Offline",
            ["AccountManager.OfflineSave"] = "Save",
            ["AccountManager.UsernameEmpty"] = "Username cannot be empty.",
            ["AccountManager.UsernameShort"] = "Username must be at least 3 characters long.",
            ["AccountManager.UsernameSpaces"] = "Username cannot contain spaces.",
            ["AccountManager.KeyFileError"] = "Key file error: {0}",
            ["AccountManager.UsernameInvalid"] = "Username contains invalid characters.",
            ["AccountManager.Microsoft"] = "Microsoft",
            ["AccountManager.SelectAccountType"] = "Select an Account Type",
            ["AccountManager.UsernameLong"] = "Username cannot be longer than 16 chars.",
            ["AccountManager.Username"] = "Username",
            ["AccountManager.CannotBeOfflineAsDeveloper"] = "You cannot use a Developers In game Nickname!",

            // AccountControl
            ["AccountControl.Login"] = "Login",
            ["AccountControl.Remove"] = "Remove",
        };

        public static string Get(string key)
        {
            Dictionary<string, string> dictionary = _currentLanguage == "tr" ? TR : EN;

            if (dictionary.TryGetValue(key, out string value))
                return value;

            return key;
        }

        public static string GetFormatted(string key, params object[] args)
        {
            string template = Get(key);
            return string.Format(template, args);
        }
    }
}
