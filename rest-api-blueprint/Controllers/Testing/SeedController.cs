using rest_api_blueprint.Constants.Identity;
using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Geo.Address;
using rest_api_blueprint.Repositories.Geo;
using rest_api_blueprint.Repositories.Identity;
using rest_api_blueprint.StaticServices;
using AutoMapper;
using LoremNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rest_api_blueprint.Entities.Social;
using rest_api_blueprint.Repositories.Social;

namespace rest_api_blueprint.Controllers.Testing
{
    [Route("testing/[controller]")]
    public class SeedController : DefaultControllerTemplate
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository<Address> _addressRepository;
        private readonly IAnnouncementRepository<Announcement> _announcementRepository;
        private readonly IMapper _mapper;

        private readonly List<CreateAddress> addressPool = new List<CreateAddress>
        {
            new CreateAddress
            {
                Company = "CenterFit 24",
                Street = "Humboldtstraße 137",
                PostalCode = "51149",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Ali Yalcin Kampfsport Center",
                Street = "Venloer Str. 182",
                PostalCode = "50823",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Kader 1 Fitnessstudio",
                Street = "Neue Weyerstraße 6",
                PostalCode = "50676",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Bushido Fitness-Studio",
                Street = "Mittelstraße 12/14b",
                PostalCode = "50672",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Fitness First",
                Street = "Schildergasse 94-96",
                PostalCode = "50667",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "FLEXX Fitness & Kurse",
                Street = "Krefelder Str. 77-83",
                PostalCode = "50670",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "AgrippaFit",
                Street = "Kämmergasse 1",
                PostalCode = "50676",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Fitness First",
                Street = "Breite Str. 80-90",
                PostalCode = "50667",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "FitnessLOFT",
                Street = "Eigelstein 80-88",
                PostalCode = "50668",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "FLEXX Fitness & Kurse",
                Street = "Weißhausstraße 20-22",
                PostalCode = "50939",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Fitness First",
                Street = "Bonner Str. 271",
                PostalCode = "50968",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Just Fit 13 Classic",
                Street = "Hohenstaufenring 30",
                PostalCode = "50674",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Fitness First",
                Street = "Luxemburger Str. 253",
                PostalCode = "50939",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Holmes Place",
                Street = "Gürzenichstraße 6-16",
                PostalCode = "50667",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "BODY STREET",
                Street = "Roonstraße 5",
                PostalCode = "50674",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "FLEXX Fitness & Kurse",
                Street = "Herzogstraße 16-20",
                PostalCode = "50667",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "«auszeit» Fitness & Gesundheit",
                Street = "Dürener Str. 75",
                PostalCode = "50931",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Mrs.Sporty Club",
                Street = "Regentenstraße 2",
                PostalCode = "51063",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "FLOW FIT EMS Training",
                Street = "Buchheimer Str. 23",
                PostalCode = "51063",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Reha Mülheimer Brücke e.V.",
                Street = "Biegerstraße 20",
                PostalCode = "51063",
                City = "Köln",
                CountryCode = "DE"
            },
            new CreateAddress
            {
                Company = "Reha Mülheimer Brücke e.V.",
                Street = "Biegerstraße 20",
                PostalCode = "51063",
                City = "Köln",
                CountryCode = "DE"
            },
        };

        public SeedController(
            IUserRepository userRepository,
            IAddressRepository<Address> addressRepository,
            IAnnouncementRepository<Announcement> announcementRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _announcementRepository = announcementRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Seed data
        /// </summary>
        [HttpPost]
        [Route("seed-data")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> SeedData()
        {
            SearchParameters parameters = new SearchParameters { PageSize = -1 };

            await SeedUsers(100);
            IEnumerable<User> users = await _userRepository.GetMultiple(parameters);

            await SeedAddresses(users);
            IEnumerable<Address> addresses = await _addressRepository.GetMultiple(parameters);

            await SeedAnnouncements(1000, users, addresses);

            return Ok();
        }

        private async Task SeedUsers(uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                string email = Lorem.Email();
                DateTime adultAge = DateTime.Now.AddYears(-18);
                DateTime maxAge = DateTime.Now.AddYears(-100);
                DateTime birthday = Lorem.DateTime(maxAge, adultAge);
                char[] genders = { 'm', 'f', 'd' };
                User user = new User
                {
                    UserName = email,
                    Email = email,
                    Gender = Lorem.Random(genders),
                    FirstName = Lorem.Words(1, true),
                    LastName = Lorem.Words(1, true),
                    Birthday = birthday,
                    AboutMe = Lorem.Sentence(3),
                    FanOf = Lorem.Words(2),
                    AvatarUri = $"https://picsum.photos/id/{Lorem.Integer(1, 999)}/1024/1012"
            };
                await _userRepository.Create(user, TextService.GeneratePassword());
            }
        }

        private async Task SeedAddresses(IEnumerable<User> users)
        {
            List<Address> addresses = new List<Address>();
            foreach (User user in users)
            {
                long countAddresses = Lorem.Number(1, 3);
                for (int i = 0; i < countAddresses; i++)
                {
                    Address address = _mapper.Map<Address>(Lorem.Random<CreateAddress>(addressPool.ToArray()));
                    address.UserId = user.Id;
                    addresses.Add(address);
                }
            }

            await _addressRepository.CreateRange(addresses);
        }

        private async Task SeedAnnouncements(uint count, IEnumerable<User> users, IEnumerable<Address> addresses)
        {
            List<Announcement> announcements = new List<Announcement>();
            for (uint i = 0; i < count; i++)
            {
                User user = Lorem.Random(users.ToArray());
                Address address = Lorem.Random(addresses.Where(a => a.UserId == user.Id).ToArray());
                Announcement announcement = new Announcement
                {
                    Title = Lorem.Words(Lorem.Integer(1, 5)),
                    AddressId = address.Id,
                    CreatorId = user.Id,
                    Bodytext = $"<p>{string.Join("</p><p>",Lorem.Paragraphs(10, 5, 10))}</p>",
                    IsPublic = Lorem.Random(new bool[] { true, false })
                };
                announcements.Add(announcement);
            }

            await _announcementRepository.CreateRange(announcements);
        }
    }
}
