﻿using ScreenMusic.Arguments;
using ScreenMusic.Domain.ApiManagement;
using ScreenMusic.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace ScreenMusic.Api.Controllers;

[Route("api/[controller]")]
public class ArtistController(IApiDataService apiDataService, IArtistService service) : BaseController<IArtistService, InputCreateArtist, InputUpdateArtist, OutputArtist, InputIdentifierArtist>(apiDataService, service) { }