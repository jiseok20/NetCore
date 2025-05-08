﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public  class AESInfo
    {
        [Required(ErrorMessage = "사용자 아이디를 입력하세요. ")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 최소 6자 이상 입력하세요. ")]
        [Display(Name = "사용자 아이디")]
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀번호를 입력하세요. ")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상 입력하세요. ")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name ="암호화 정보")]
        public string EncUserInfo { get; set; }

        [Display(Name ="복호화 정보")]
        public string DecUserInfo {  get; set; }
    }
}
