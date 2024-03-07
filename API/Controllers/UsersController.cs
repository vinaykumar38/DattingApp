using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Helpers;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Http;






namespace API.Controllers
{

    [Authorize]
    
    public class UsersController:BaseApiController
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;
        
        
        

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
      
           
            
        }
         [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){
            
           var users = await _userRepository.GetMembersAsync();
            
           return Ok(users);
            
            
        }


         [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username){
           return await _userRepository.GetMemberAsync(username);        
        
        }
         
        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
          
          
           var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());


           if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }





        //  [AllowAnonymous]
        // [HttpPut]
        // public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        // {   
        //     var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //     // var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        //     var user = await _userRepository.GetUserByUsernameAsync(username);

        //    if (user == null) return NotFound();

        //     _mapper.Map(memberUpdateDto, user);

        //     if (await _userRepository.SaveAllAsync()) return NoContent();

        //     return BadRequest("Failed to update user");
        // }



//manual without jwt token

//         [AllowAnonymous]

//         [HttpPut("{username}")]
// public async Task<ActionResult> UpdateUser(string username, MemberUpdateDto memberUpdateDto)
// {   
//     var user = await _userRepository.GetUserByUsernameAsync(username);

//     if (user == null) return NotFound();

//     _mapper.Map(memberUpdateDto, user);

//     if (await _userRepository.SaveAllAsync()) return NoContent();

//     return BadRequest("Failed to update user");
// }


       



         [AllowAnonymous]
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);
            return BadRequest("Problem adding photo");
        }





       






      


        
        


    }
}