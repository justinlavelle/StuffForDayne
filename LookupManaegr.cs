#region UsingStatements

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using log4net;
using Translations.DAL;
using Translations.Domain.Interfaces;
using Translations.Managers.Interfaces;
using Translations.Managers.Mappers;

#endregion

namespace Translations.Managers
{
    public class LookupManager : BaseManager, ILookupManager
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Gets all users.
        /// </summary>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllUsers()
        {
            using (var context = new TranslationsEntities())
            {
                IOrderedQueryable<User> s = from t in context.Users
                    orderby t.UserName
                    select t;
                return s.MapUserToDomain();
            }
        }

        /// <summary>
        ///     Gets all locales.
        /// </summary>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllLocales()
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Locale> s = from ll in context.Locales
                    join c in context.Countries on ll.CountryId equals c.Id
                    join l in context.Languages on ll.LanguageId equals l.Id
                    where ll.isAvailable == true
                    select ll;

                // Order the results by lang, then country
                s = s.OrderBy(locale => locale.Language.IsoCode).ThenBy(locale => locale.Country.IsoCode);

                return s.MapLocaleToDomain();
            }
        }

        /// <summary>
        ///     Gets all platforms.
        /// </summary>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllPlatforms()
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Platform> s = from t in context.Platforms select t;
                return s.MapPlatformToDomain();
            }
        }

        /// <summary>
        ///     Gets the locales for project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public List<ILookupDomainModel> GetLocalesForProject(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Locale> projectLocales = (from p in context.Projects
                    join pd in context.ProjectDetails on p.Id equals pd.ProjectId
                    join ll in context.Locales on pd.LocaleId equals ll.Id
                    join c in context.Countries on ll.CountryId equals c.Id
                    join l in context.Languages on ll.LanguageId equals l.Id
                    where p.Id == projectId
                    select pd.Locale).Distinct();

                return projectLocales.MapLocaleToDomain();
            }
        }

        /// <summary>
        ///     Gets the platforms for project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public List<ILookupDomainModel> GetPlatformsForProject(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Platform> projectPlatforms = (from p in context.Projects
                    join pd in context.ProjectDetails on p.Id equals pd.ProjectId
                    join pp in context.Platforms on pd.PlatformId equals pp.Id
                    where p.Id == projectId
                    select pd.Platform).Distinct();

                return projectPlatforms.MapPlatformToDomain();
            }
        }

        /// <summary>
        ///     Gets the project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public ILookupDomainModel GetProject(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                Project project = (from t in context.Projects where t.Id == projectId select t).FirstOrDefault();

                return project != null ? project.MapProjectToDomain() : null;
            }
        }

        /// <summary>
        ///     Gets the platform.
        /// </summary>
        /// <param name="platformId">The platform identifier.</param>
        /// <returns></returns>
        public ILookupDomainModel GetPlatform(int platformId)
        {
            using (var context = new TranslationsEntities())
            {
                Platform platform = (from t in context.Platforms where t.Id == platformId select t).FirstOrDefault();

                return platform != null ? platform.MapPlatformToDomain() : null;
            }
        }

        /// <summary>
        ///     Gets the locale.
        /// </summary>
        /// <param name="localeId">The locale identifier.</param>
        /// <returns></returns>
        public ILookupDomainModel GetLocale(int localeId)
        {
            using (var context = new TranslationsEntities())
            {
                Locale locale = (from t in context.Locales where t.Id == localeId select t).FirstOrDefault();

                return locale != null ? locale.MapLocaleToDomain() : null;
            }
        }

        /// <summary>
        ///     Gets the file status.
        /// </summary>
        /// <param name="fileStatusId">The file status identifier.</param>
        /// <returns></returns>
        public ILookupDomainModel GetFileStatus(int fileStatusId)
        {
            using (var context = new TranslationsEntities())
            {
                FileStatu statusName =
                    (from t in context.FileStatus where t.Id == fileStatusId select t).FirstOrDefault();

                return statusName != null ? statusName.MapFileStatusToDomain() : null;
            }
        }

        public void UpdateAvailableLocales(int id)
        {
            using (var context = new TranslationsEntities())
            {
                Locale locale = (context.Locales.Where(lo => lo.Id == id)).FirstOrDefault();
                if (locale != null) locale.isAvailable = false;
                context.SaveChanges();
            }
        }

        /// <summary>
        ///     Gets all users.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllUsers(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<User> s = from up in context.UserProjects
                    join u in context.Users on up.UserId equals u.Id
                    where up.ProjectId == projectId
                    select u;
                return s.MapUserToDomain();
            }
        }

        /// <summary>
        ///     Gets all locales.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllLocales(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Locale> s = (from ll in context.Locales
                    join c in context.Countries on ll.CountryId equals c.Id
                    join l in context.Languages on ll.LanguageId equals l.Id
                    join pd in context.ProjectDetails on ll.Id equals pd.LocaleId
                    where pd.ProjectId == projectId && pd.LocaleId != 34
                    select ll).Distinct();

                return s.MapLocaleToDomain();
            }
        }

        /// <summary>
        ///     Gets all platforms.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public List<ILookupDomainModel> GetAllPlatforms(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<Platform> s = (from p in context.Platforms
                    join pd in context.ProjectDetails on p.Id equals pd.PlatformId
                    where pd.ProjectId == projectId
                    select p).Distinct();
                return s.MapPlatformToDomain();
            }
        }

        /// <summary>
        ///     Gets the owner.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        public ILookupDomainModel GetOwner(int ownerId)
        {
            using (var context = new TranslationsEntities())
            {
                User owner = (from t in context.Users where t.Id == ownerId select t).FirstOrDefault();

                return owner != null ? owner.MapUserToDomain() : null;
            }
        }

        /// <summary>
        ///     Gets the file count.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public int GetFileCount(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                int files = (from p in context.Projects
                    join pd in context.ProjectDetails on p.Id equals pd.ProjectId
                    join sf in context.SourceFiles on pd.Id equals sf.ProjectDetailId
                    where pd.ProjectId == projectId
                    select sf).Count();


                return files;
            }
        }

        /// <summary>
        ///     Gets the file type identifier.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        public int GetFileTypeId(string fileType)
        {
            using (var context = new TranslationsEntities())
            {
                int fileTypeId = (from ft in context.FileTypes
                    where ft.FileType1 == fileType
                    select ft.Id).FirstOrDefault();

                return fileTypeId != 0 ? fileTypeId : 0;
            }
        }

        /// <summary>
        ///     Gets the platform count.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public int GetPlatformCount(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<int> platforms = (from pd in context.ProjectDetails
                    where pd.ProjectId == projectId
                    select pd.PlatformId).Distinct();

                return platforms.Count();
            }
        }

        /// <summary>
        ///     Gets the locale count.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public int GetLocaleCount(int projectId)
        {
            using (var context = new TranslationsEntities())
            {
                IQueryable<int> locales = (from pd in context.ProjectDetails
                    where pd.ProjectId == projectId && pd.LocaleId != 34
                    select pd.LocaleId).Distinct();

                return locales.Count();
            }
        }

        /// <summary>
        ///     Returns a comma-separated list of the available platforms for the project ID supplied
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public string GetPlatformNamesFormatted(int projectId)
        {
            List<ILookupDomainModel> platforms = GetPlatformsForProject(projectId);
            return string.Join(", ", from platform in platforms select platform.DisplayText);
        }

        public List<Locale> GetSourceLocales()
        {
            using (var context = new TranslationsEntities())
            {
                var sourceLocales = new[] {34, 33};
                return
                    new List<Locale>(
                        context.Locales.Where(x => sourceLocales.Contains(x.Id))
                            .Include(i => i.Country)
                            .Include(j => j.Language)).ToList();
            }
        }
    }
}