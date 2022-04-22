using Note.BusinessLayer.Abstract;
using Note.BusinessLayer.Results;
using Note.Common.Helpers;
using Note.DataAccessLayer.EF;
using Note.Domain;
using Note.Domain.Messages;
using Note.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.BusinessLayer
{
    public class UserManager:ManagerBase<NotUser>
    {
        public BusinessLayerResult<NotUser> RegisterUser(RegisterViewModel data)
        {
            NotUser user=Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı");
                }
            }
            else
            {
                int dbResult=base.Insert(new NotUser()
                {
                    Username=data.Username,
                    Email=data.Email,
                    ProfileImageFilename="User_Profile.png",
                    Password=data.Password,
                    ActivateGuid=Guid.NewGuid(),
                    IsActive =false,
                    IsAdmin=false
                    
                });
                if (dbResult > 0)
                {
                    res.Result =Find(x => x.Email == data.Email && x.Username == data.Username);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br> Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    MailHelper.SendMail(body, res.Result.Email,"Hesap Aktifleştirme",true);

                   
                }
            }
            return res;

        }

        public BusinessLayerResult<NotUser> GetUserById(object ıd)
        {
            throw new NotImplementedException();
        }

        public BusinessLayerResult<NotUser> GetUserById(int id)
        {
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            res.Result =Find(x => x.Id == id);
            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<NotUser> LoginUser(LoginViewModel data)
        {
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            res.Result=Find(x => x.Username == data.Username && x.Password == data.Password);

         
            if(res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                    


                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı yada şifre uyuşmuyor.");
            }
            return res;


        }

        public BusinessLayerResult<NotUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            res.Result =Find(x => x.ActivateGuid == activateId);

            if(res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;

                }
                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı.");

            }
            return res;
        }

        public BusinessLayerResult<NotUser> RemoveUserById(int id)
        {
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            NotUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<NotUser> UpdateProfile(NotUser data)
        {
            NotUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }

        public new BusinessLayerResult<NotUser> Insert(NotUser data)
        {
            //Method hiding
            NotUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı");
                }
            }
            else
            {
                res.Result.ProfileImageFilename = "User_Profile.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
                }
                
            }
            return res;

        }

        public new BusinessLayerResult<NotUser> Update(NotUser data)
        {
            NotUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<NotUser> res = new BusinessLayerResult<NotUser>();
            res.Result = data;
            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;


            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }
    }
}
