using Microsoft.EntityFrameworkCore;
using CVEnhancer.Models;
using CVEnhancer.Utils;

namespace CVEnhancer.Data
{
    public static class DbSeeder
    {
        public static void SeedDatabase(DbContext context)
        {
            // SprawdŸ czy u¿ytkownicy ju¿ istniej¹
            var existingUser = context.Set<User>().FirstOrDefault();
            if (existingUser != null)
                return; // Dane ju¿ istniej¹

            // 1. Seed Skill Categories
            var programmingCat = new SkillCategory { Name = "Programowanie" };
            var frameworksCat = new SkillCategory { Name = "Frameworki" };
            var databaseCat = new SkillCategory { Name = "Bazy danych" };
            var cloudCat = new SkillCategory { Name = "Cloud & DevOps" };
            var toolsCat = new SkillCategory { Name = "Narzêdzia" };
            var softSkillsCat = new SkillCategory { Name = "Soft Skills" };
            var frontendCat = new SkillCategory { Name = "Frontend" };
            var osCat = new SkillCategory { Name = "Systemy operacyjne" };

            context.Set<SkillCategory>().AddRange(
                programmingCat, frameworksCat, databaseCat, cloudCat, toolsCat, softSkillsCat, frontendCat, osCat
            );
            context.SaveChanges();

            // 2. Seed Skills
            var skills = SeedSkills(context, programmingCat, frameworksCat, databaseCat, cloudCat, toolsCat, softSkillsCat, frontendCat, osCat);

            // 3. Seed Users
            var (janKowalski, annaNowak) = SeedUsers(context);

            // 4. Seed Work Experiences
            SeedWorkExperiences(context, janKowalski, annaNowak, skills);

            // 5. Seed Projects
            SeedProjects(context, janKowalski, annaNowak, skills);

            // 6. Seed Certificates
            SeedCertificates(context, janKowalski, annaNowak, skills);

            // 7. Seed Education
            SeedEducation(context, janKowalski, annaNowak, skills);
        }

        private static Dictionary<string, Skill> SeedSkills(
            DbContext context,
            SkillCategory programmingCat,
            SkillCategory frameworksCat,
            SkillCategory databaseCat,
            SkillCategory cloudCat,
            SkillCategory toolsCat,
            SkillCategory softSkillsCat,
            SkillCategory frontendCat,
            SkillCategory osCat)
        {
            var skillDict = new Dictionary<string, Skill>();

            // Programming Languages
            skillDict["csharp"] = new Skill { Name = "C#", Category = programmingCat };
            skillDict["javascript"] = new Skill { Name = "JavaScript", Category = programmingCat };
            skillDict["typescript"] = new Skill { Name = "TypeScript", Category = programmingCat };
            skillDict["python"] = new Skill { Name = "Python", Category = programmingCat };
            skillDict["java"] = new Skill { Name = "Java", Category = programmingCat };

            // Frameworks
            skillDict["dotnet"] = new Skill { Name = ".NET", Category = frameworksCat };
            skillDict["dotnetCore"] = new Skill { Name = ".NET Core", Category = frameworksCat };
            skillDict["aspnet"] = new Skill { Name = "ASP.NET Core", Category = frameworksCat };
            skillDict["aspnetMvc"] = new Skill { Name = "ASP.NET MVC", Category = frameworksCat };
            skillDict["maui"] = new Skill { Name = ".NET MAUI", Category = frameworksCat };
            skillDict["react"] = new Skill { Name = "React", Category = frameworksCat };
            skillDict["angular"] = new Skill { Name = "Angular", Category = frameworksCat };
            skillDict["blazor"] = new Skill { Name = "Blazor", Category = frameworksCat };
            skillDict["entityFramework"] = new Skill { Name = "Entity Framework", Category = frameworksCat };
            skillDict["spring"] = new Skill { Name = "Spring", Category = frameworksCat };
            skillDict["microservices"] = new Skill { Name = "Microservices", Category = frameworksCat };
            skillDict["restApi"] = new Skill { Name = "REST API", Category = frameworksCat };
            skillDict["webService"] = new Skill { Name = "WebService", Category = frameworksCat };
            skillDict["winForms"] = new Skill { Name = "WinForms", Category = frameworksCat };

            // Databases
            skillDict["sqlServer"] = new Skill { Name = "SQL Server", Category = databaseCat };
            skillDict["sql"] = new Skill { Name = "SQL", Category = databaseCat };
            skillDict["tsql"] = new Skill { Name = "T-SQL", Category = databaseCat };
            skillDict["plsql"] = new Skill { Name = "PL/SQL", Category = databaseCat };
            skillDict["postgresql"] = new Skill { Name = "PostgreSQL", Category = databaseCat };
            skillDict["sqlite"] = new Skill { Name = "SQLite", Category = databaseCat };
            skillDict["mongodb"] = new Skill { Name = "MongoDB", Category = databaseCat };
            skillDict["redis"] = new Skill { Name = "Redis", Category = databaseCat };
            skillDict["snowflake"] = new Skill { Name = "Snowflake", Category = databaseCat };

            // Cloud & DevOps
            skillDict["azure"] = new Skill { Name = "Azure", Category = cloudCat };
            skillDict["azureCloud"] = new Skill { Name = "Azure Cloud", Category = cloudCat };
            skillDict["azureDevOps"] = new Skill { Name = "Azure DevOps", Category = cloudCat };
            skillDict["aws"] = new Skill { Name = "AWS", Category = cloudCat };
            skillDict["docker"] = new Skill { Name = "Docker", Category = cloudCat };
            skillDict["kubernetes"] = new Skill { Name = "Kubernetes", Category = cloudCat };
            skillDict["openshift"] = new Skill { Name = "OpenShift", Category = cloudCat };
            skillDict["cicd"] = new Skill { Name = "CI/CD", Category = cloudCat };
            skillDict["kafka"] = new Skill { Name = "Kafka", Category = cloudCat };
            skillDict["rabbitmq"] = new Skill { Name = "RabbitMQ", Category = cloudCat };

            // Tools
            skillDict["git"] = new Skill { Name = "Git", Category = toolsCat };
            skillDict["visualStudio"] = new Skill { Name = "Visual Studio", Category = toolsCat };
            skillDict["vscode"] = new Skill { Name = "VS Code", Category = toolsCat };
            skillDict["postman"] = new Skill { Name = "Postman", Category = toolsCat };
            skillDict["jira"] = new Skill { Name = "Jira", Category = toolsCat };
            skillDict["githubCopilot"] = new Skill { Name = "GitHub Copilot", Category = toolsCat };

            // Frontend
            skillDict["html"] = new Skill { Name = "HTML", Category = frontendCat };
            skillDict["css"] = new Skill { Name = "CSS", Category = frontendCat };
            skillDict["scss"] = new Skill { Name = "SCSS", Category = frontendCat };

            // Operating Systems
            skillDict["linux"] = new Skill { Name = "Linux", Category = osCat };
            skillDict["windows"] = new Skill { Name = "Windows", Category = osCat };

            // Soft Skills
            skillDict["agile"] = new Skill { Name = "Agile", Category = softSkillsCat };
            skillDict["scrum"] = new Skill { Name = "Scrum", Category = softSkillsCat };
            skillDict["teamwork"] = new Skill { Name = "Praca zespo³owa", Category = softSkillsCat };
            skillDict["communication"] = new Skill { Name = "Komunikacja", Category = softSkillsCat };

            context.Set<Skill>().AddRange(skillDict.Values);
            context.SaveChanges();

            return skillDict;
        }

        private static (User janKowalski, User annaNowak) SeedUsers(DbContext context)
        {
            var janKowalski = new User
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                ProfilePicture = new ProfilePicture { Picture = ImageByteConverter.defaultImage },
                Email = "jan.kowalski@example.com",
                PhoneNumber = "+48 123 456 789",
                LinkedInUrl = "https://linkedin.com/in/jankowalski",
                GitHubUrl = "https://github.com/jankowalski",
                JobTitle = "Senior .NET Developer",
                ProfessionalSummary = "Doœwiadczony programista .NET z 5-letnim sta¿em w tworzeniu aplikacji enterprise."
            };

            var annaNowak = new User
            {
                FirstName = "Anna",
                LastName = "Nowak",
                ProfilePicture = new ProfilePicture { Picture = ImageByteConverter.defaultImage },
                Email = "anna.nowak@example.com",
                PhoneNumber = "+48 987 654 321",
                LinkedInUrl = "https://linkedin.com/in/annanowak",
                GitHubUrl = "https://github.com/annanowak",
                JobTitle = "Full Stack Developer",
                ProfessionalSummary = "Wszechstronna programistka z pasj¹ do tworzenia nowoczesnych aplikacji webowych i mobilnych."
            };

            context.Set<User>().AddRange(janKowalski, annaNowak);
            context.SaveChanges();

            return (janKowalski, annaNowak);
        }

        private static void SeedWorkExperiences(DbContext context, User janKowalski, User annaNowak, Dictionary<string, Skill> skills)
        {
            // Jan Kowalski Work Experiences
            var janWork1 = new WorkExperience
            {
                User = janKowalski,
                CompanyName = "TechCorp Solutions",
                JobTitle = "Senior .NET Developer",
                StartDate = new DateTime(2021, 3, 1),
                EndDate = null,
                Description = "Projektowanie i implementacja skalowalnych aplikacji enterprise w architekturze mikrous³ug. Mentoring m³odszych programistów.",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["dotnetCore"], skills["aspnet"], 
                    skills["azure"], skills["azureCloud"], skills["sqlServer"], skills["sql"], 
                    skills["tsql"], skills["docker"], skills["kubernetes"], skills["microservices"], 
                    skills["restApi"], skills["entityFramework"] 
                }
            };

            var janWork2 = new WorkExperience
            {
                User = janKowalski,
                CompanyName = "Digital Systems Sp. z o.o.",
                JobTitle = ".NET Developer",
                StartDate = new DateTime(2019, 6, 1),
                EndDate = new DateTime(2021, 2, 28),
                Description = "Rozwój systemów webowych dla klientów z sektora bankowego. Implementacja RESTful API i integracji systemowych.",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["aspnetMvc"], 
                    skills["sqlServer"], skills["entityFramework"], skills["git"], skills["agile"], 
                    skills["restApi"], skills["windows"] 
                }
            };

            var janWork3 = new WorkExperience
            {
                User = janKowalski,
                CompanyName = "StartupHub",
                JobTitle = "Junior .NET Developer",
                StartDate = new DateTime(2018, 1, 15),
                EndDate = new DateTime(2019, 5, 31),
                Description = "Wsparcie w rozwoju aplikacji webowych. Nauka best practices i wzorców projektowych.",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["sqlServer"], 
                    skills["git"], skills["html"], skills["css"], skills["javascript"] 
                }
            };

            // Anna Nowak Work Experiences
            var annaWork1 = new WorkExperience
            {
                User = annaNowak,
                CompanyName = "WebDev Studio",
                JobTitle = "Full Stack Developer",
                StartDate = new DateTime(2020, 9, 1),
                EndDate = null,
                Description = "Tworzenie responsywnych aplikacji webowych i mobilnych. Praca z React, Angular oraz .NET Core.",
                Skills = new List<Skill> { 
                    skills["javascript"], skills["typescript"], skills["react"], skills["angular"], 
                    skills["dotnet"], skills["dotnetCore"], skills["aspnet"], skills["postgresql"], 
                    skills["docker"], skills["html"], skills["css"], skills["scss"], 
                    skills["restApi"], skills["git"] 
                }
            };

            var annaWork2 = new WorkExperience
            {
                User = annaNowak,
                CompanyName = "Mobile Solutions Inc.",
                JobTitle = "Frontend Developer",
                StartDate = new DateTime(2019, 3, 1),
                EndDate = new DateTime(2020, 8, 31),
                Description = "Rozwój interfejsów u¿ytkownika dla aplikacji mobilnych i webowych. Optymalizacja wydajnoœci.",
                Skills = new List<Skill> { 
                    skills["javascript"], skills["react"], skills["typescript"], skills["git"], 
                    skills["agile"], skills["html"], skills["css"], skills["scss"] 
                }
            };

            var annaWork3 = new WorkExperience
            {
                User = annaNowak,
                CompanyName = "CodeFactory",
                JobTitle = "Junior Frontend Developer",
                StartDate = new DateTime(2017, 7, 1),
                EndDate = new DateTime(2019, 2, 28),
                Description = "Implementacja projektów graficznych w HTML, CSS i JavaScript. Wspó³praca z zespo³em designerów.",
                Skills = new List<Skill> { 
                    skills["javascript"], skills["react"], skills["git"], 
                    skills["html"], skills["css"] 
                }
            };

            context.Set<WorkExperience>().AddRange(janWork1, janWork2, janWork3, annaWork1, annaWork2, annaWork3);
            context.SaveChanges();
        }

        private static void SeedProjects(DbContext context, User janKowalski, User annaNowak, Dictionary<string, Skill> skills)
        {
            // Jan Kowalski Projects
            var janProject1 = new Project
            {
                User = janKowalski,
                Name = "E-Commerce Platform",
                Description = "Platforma e-commerce z systemem p³atnoœci, zarz¹dzaniem produktami i panelem administracyjnym.",
                ProjectUrl = "https://github.com/jankowalski/ecommerce-platform",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["blazor"], 
                    skills["sqlServer"], skills["azure"], skills["entityFramework"], 
                    skills["restApi"], skills["html"], skills["css"] 
                }
            };

            var janProject2 = new Project
            {
                User = janKowalski,
                Name = "Task Management System",
                Description = "System do zarz¹dzania zadaniami dla zespo³ów programistycznych z integracj¹ Jira.",
                ProjectUrl = "https://github.com/jankowalski/task-manager",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["react"], 
                    skills["postgresql"], skills["docker"], skills["restApi"], skills["typescript"] 
                }
            };

            var janProject3 = new Project
            {
                User = janKowalski,
                Name = "CV Generator API",
                Description = "RESTful API do generowania CV w ró¿nych formatach (PDF, DOCX, JSON).",
                ProjectUrl = "https://github.com/jankowalski/cv-generator",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], 
                    skills["sqlite"], skills["docker"], skills["restApi"] 
                }
            };

            var janProject4 = new Project
            {
                User = janKowalski,
                Name = "Microservices Architecture Demo",
                Description = "Demonstracja architektury mikrous³ug z API Gateway, RabbitMQ i Docker.",
                ProjectUrl = "https://github.com/jankowalski/microservices-demo",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["docker"], 
                    skills["kubernetes"], skills["redis"], skills["mongodb"], skills["rabbitmq"], 
                    skills["kafka"], skills["microservices"], skills["restApi"] 
                }
            };

            var janProject5 = new Project
            {
                User = janKowalski,
                Name = "Real-Time Chat Application",
                Description = "Aplikacja czatu w czasie rzeczywistym wykorzystuj¹ca SignalR i React.",
                ProjectUrl = "https://github.com/jankowalski/realtime-chat",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["dotnet"], skills["aspnet"], skills["react"], 
                    skills["sqlServer"], skills["azure"], skills["typescript"], 
                    skills["html"], skills["css"] 
                }
            };

            // Anna Nowak Projects
            var annaProject1 = new Project
            {
                User = annaNowak,
                Name = "Social Media Dashboard",
                Description = "Dashboard do zarz¹dzania kontami social media z analiz¹ statystyk.",
                ProjectUrl = "https://github.com/annanowak/social-dashboard",
                Skills = new List<Skill> { 
                    skills["react"], skills["typescript"], skills["dotnet"], skills["postgresql"], 
                    skills["docker"], skills["html"], skills["css"], skills["scss"], skills["restApi"] 
                }
            };

            var annaProject2 = new Project
            {
                User = annaNowak,
                Name = "Weather App MAUI",
                Description = "Multiplatformowa aplikacja pogodowa napisana w .NET MAUI.",
                ProjectUrl = "https://github.com/annanowak/weather-app",
                Skills = new List<Skill> { 
                    skills["csharp"], skills["maui"], skills["dotnet"], 
                    skills["sqlite"], skills["restApi"] 
                }
            };

            var annaProject3 = new Project
            {
                User = annaNowak,
                Name = "Recipe Finder",
                Description = "Aplikacja webowa do wyszukiwania przepisów kulinarnych z integracj¹ API.",
                ProjectUrl = "https://github.com/annanowak/recipe-finder",
                Skills = new List<Skill> { 
                    skills["javascript"], skills["react"], skills["dotnet"], skills["aspnet"], 
                    skills["mongodb"], skills["html"], skills["css"], skills["restApi"] 
                }
            };

            var annaProject4 = new Project
            {
                User = annaNowak,
                Name = "Portfolio Generator",
                Description = "Generator stron portfolio dla programistów z edytorem drag-and-drop.",
                ProjectUrl = "https://github.com/annanowak/portfolio-gen",
                Skills = new List<Skill> { 
                    skills["typescript"], skills["angular"], skills["dotnet"], 
                    skills["postgresql"], skills["azure"], skills["html"], 
                    skills["css"], skills["scss"] 
                }
            };

            var annaProject5 = new Project
            {
                User = annaNowak,
                Name = "Fitness Tracker PWA",
                Description = "Progressive Web App do œledzenia aktywnoœci fizycznej i diety.",
                ProjectUrl = "https://github.com/annanowak/fitness-tracker",
                Skills = new List<Skill> { 
                    skills["react"], skills["typescript"], skills["python"], 
                    skills["mongodb"], skills["docker"], skills["html"], 
                    skills["css"], skills["scss"] 
                }
            };

            context.Set<Project>().AddRange(
                janProject1, janProject2, janProject3, janProject4, janProject5,
                annaProject1, annaProject2, annaProject3, annaProject4, annaProject5
            );
            context.SaveChanges();
        }

        private static void SeedCertificates(DbContext context, User janKowalski, User annaNowak, Dictionary<string, Skill> skills)
        {
            // Jan Kowalski Certificates
            var janCert1 = new Certificate
            {
                User = janKowalski,
                Name = "Microsoft Certified: Azure Developer Associate",
                IssuingOrganization = "Microsoft",
                IssueDate = new DateTime(2022, 6, 15),
                ExpirationDate = new DateTime(2024, 6, 15),
                CredentialId = "AZ-204-2022-JK789",
                CredentialUrl = "https://learn.microsoft.com/credentials/certifications/azure-developer/",
                Skills = new List<Skill> { 
                    skills["azure"], skills["azureCloud"], skills["csharp"], 
                    skills["dotnet"], skills["docker"], skills["restApi"] 
                }
            };

            var janCert2 = new Certificate
            {
                User = janKowalski,
                Name = "Certified Kubernetes Application Developer",
                IssuingOrganization = "Cloud Native Computing Foundation",
                IssueDate = new DateTime(2023, 3, 10),
                ExpirationDate = new DateTime(2026, 3, 10),
                CredentialId = "CKAD-2023-456",
                CredentialUrl = "https://www.cncf.io/certification/ckad/",
                Skills = new List<Skill> { 
                    skills["kubernetes"], skills["docker"], skills["cicd"], 
                    skills["microservices"], skills["linux"] 
                }
            };

            var janCert3 = new Certificate
            {
                User = janKowalski,
                Name = "Professional Scrum Master I",
                IssuingOrganization = "Scrum.org",
                IssueDate = new DateTime(2021, 9, 5),
                ExpirationDate = null,
                CredentialId = "PSM-I-2021-JK123",
                CredentialUrl = "https://www.scrum.org/assessments/professional-scrum-master-i-certification",
                Skills = new List<Skill> { skills["scrum"], skills["agile"], skills["teamwork"] }
            };

            var janCert4 = new Certificate
            {
                User = janKowalski,
                Name = "Microsoft Certified: DevOps Engineer Expert",
                IssuingOrganization = "Microsoft",
                IssueDate = new DateTime(2023, 8, 20),
                ExpirationDate = new DateTime(2025, 8, 20),
                CredentialId = "AZ-400-2023-JK456",
                CredentialUrl = "https://learn.microsoft.com/credentials/certifications/devops-engineer/",
                Skills = new List<Skill> { 
                    skills["azure"], skills["azureDevOps"], skills["cicd"], 
                    skills["docker"], skills["kubernetes"], skills["git"] 
                }
            };

            var janCert5 = new Certificate
            {
                User = janKowalski,
                Name = "Entity Framework Core Mastery",
                IssuingOrganization = "Pluralsight",
                IssueDate = new DateTime(2022, 11, 30),
                ExpirationDate = null,
                CredentialId = "PS-EF-2022-789",
                Skills = new List<Skill> { 
                    skills["entityFramework"], skills["csharp"], 
                    skills["dotnet"], skills["sqlServer"], skills["sql"] 
                }
            };

            // Anna Nowak Certificates
            var annaCert1 = new Certificate
            {
                User = annaNowak,
                Name = "Meta Front-End Developer Professional Certificate",
                IssuingOrganization = "Meta (Coursera)",
                IssueDate = new DateTime(2022, 4, 12),
                ExpirationDate = null,
                CredentialId = "META-FE-2022-AN456",
                CredentialUrl = "https://www.coursera.org/professional-certificates/meta-front-end-developer",
                Skills = new List<Skill> { 
                    skills["react"], skills["javascript"], skills["typescript"], 
                    skills["git"], skills["html"], skills["css"] 
                }
            };

            var annaCert2 = new Certificate
            {
                User = annaNowak,
                Name = "AWS Certified Developer - Associate",
                IssuingOrganization = "Amazon Web Services",
                IssueDate = new DateTime(2023, 1, 25),
                ExpirationDate = new DateTime(2026, 1, 25),
                CredentialId = "AWS-DEV-2023-AN789",
                CredentialUrl = "https://aws.amazon.com/certification/certified-developer-associate/",
                Skills = new List<Skill> { 
                    skills["aws"], skills["docker"], skills["cicd"], 
                    skills["python"], skills["linux"] 
                }
            };

            var annaCert3 = new Certificate
            {
                User = annaNowak,
                Name = "Angular - The Complete Guide",
                IssuingOrganization = "Udemy",
                IssueDate = new DateTime(2021, 7, 18),
                ExpirationDate = null,
                CredentialId = "UC-ANGULAR-2021-456",
                Skills = new List<Skill> { 
                    skills["angular"], skills["typescript"], skills["javascript"], 
                    skills["html"], skills["css"], skills["scss"] 
                }
            };

            var annaCert4 = new Certificate
            {
                User = annaNowak,
                Name = "MongoDB Developer Certification",
                IssuingOrganization = "MongoDB University",
                IssueDate = new DateTime(2022, 10, 8),
                ExpirationDate = new DateTime(2025, 10, 8),
                CredentialId = "MONGO-DEV-2022-AN123",
                CredentialUrl = "https://university.mongodb.com/certification",
                Skills = new List<Skill> { skills["mongodb"], skills["javascript"], skills["python"] }
            };

            var annaCert5 = new Certificate
            {
                User = annaNowak,
                Name = "Professional Scrum Product Owner I",
                IssuingOrganization = "Scrum.org",
                IssueDate = new DateTime(2023, 5, 14),
                ExpirationDate = null,
                CredentialId = "PSPO-I-2023-AN789",
                CredentialUrl = "https://www.scrum.org/assessments/professional-scrum-product-owner-i-certification",
                Skills = new List<Skill> { skills["scrum"], skills["agile"], skills["communication"] }
            };

            context.Set<Certificate>().AddRange(
                janCert1, janCert2, janCert3, janCert4, janCert5,
                annaCert1, annaCert2, annaCert3, annaCert4, annaCert5
            );
            context.SaveChanges();
        }

        private static void SeedEducation(DbContext context, User janKowalski, User annaNowak, Dictionary<string, Skill> skills)
        {
            // Jan Kowalski Education
            var janEdu1 = new Education
            {
                User = janKowalski,
                InstitutionName = "Politechnika Warszawska",
                Degree = "Magister in¿ynier",
                FieldOfStudy = "Informatyka",
                StartDate = new DateTime(2013, 10, 1),
                EndDate = new DateTime(2018, 6, 30),
                Skills = new List<Skill> { 
                    skills["csharp"], skills["java"], skills["python"], skills["sqlServer"], 
                    skills["sql"], skills["git"], skills["agile"], skills["html"], 
                    skills["css"], skills["javascript"] 
                }
            };

            var janEdu2 = new Education
            {
                User = janKowalski,
                InstitutionName = "Akademia Mikrosoft",
                Degree = "Certyfikat",
                FieldOfStudy = "Cloud Computing i Azure",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 12, 31),
                Skills = new List<Skill> { 
                    skills["azure"], skills["azureCloud"], skills["azureDevOps"], 
                    skills["docker"], skills["kubernetes"], skills["cicd"] 
                }
            };

            var janEdu3 = new Education
            {
                User = janKowalski,
                InstitutionName = "Pluralsight",
                Degree = "Œcie¿ka kursowa",
                FieldOfStudy = ".NET Microservices Architecture",
                StartDate = new DateTime(2022, 6, 1),
                EndDate = new DateTime(2022, 9, 30),
                Skills = new List<Skill> { 
                    skills["dotnet"], skills["dotnetCore"], skills["aspnet"], skills["docker"], 
                    skills["kubernetes"], skills["redis"], skills["mongodb"], skills["microservices"], 
                    skills["restApi"], skills["rabbitmq"] 
                }
            };

            // Anna Nowak Education
            var annaEdu1 = new Education
            {
                User = annaNowak,
                InstitutionName = "Uniwersytet Jagielloñski",
                Degree = "Licencjat",
                FieldOfStudy = "Informatyka Stosowana",
                StartDate = new DateTime(2014, 10, 1),
                EndDate = new DateTime(2017, 6, 30),
                Skills = new List<Skill> { 
                    skills["javascript"], skills["python"], skills["java"], 
                    skills["git"], skills["html"], skills["css"], skills["sql"] 
                }
            };

            var annaEdu2 = new Education
            {
                User = annaNowak,
                InstitutionName = "Coursera",
                Degree = "Specjalizacja",
                FieldOfStudy = "Full-Stack Web Development",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 8, 31),
                Skills = new List<Skill> { 
                    skills["react"], skills["angular"], skills["javascript"], skills["typescript"], 
                    skills["dotnet"], skills["mongodb"], skills["html"], skills["css"], 
                    skills["scss"], skills["restApi"] 
                }
            };

            var annaEdu3 = new Education
            {
                User = annaNowak,
                InstitutionName = "Udemy",
                Degree = "Kurs",
                FieldOfStudy = ".NET MAUI - Cross Platform Development",
                StartDate = new DateTime(2023, 3, 1),
                EndDate = new DateTime(2023, 5, 31),
                Skills = new List<Skill> { 
                    skills["maui"], skills["csharp"], skills["dotnet"], 
                    skills["sqlite"], skills["restApi"] 
                }
            };

            context.Set<Education>().AddRange(
                janEdu1, janEdu2, janEdu3,
                annaEdu1, annaEdu2, annaEdu3
            );
            context.SaveChanges();
        }
    }
}
