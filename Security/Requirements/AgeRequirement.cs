using Microsoft.AspNetCore.Authorization;

namespace MVC_1.Security.Requirement
{
    public class AgeRequirement:IAuthorizationRequirement
    {
        public int _Minyear{get;}
        public int _Maxyear{get;}
        public AgeRequirement(int MinYear,int MaxYear)
        {
            _Minyear=MinYear;
            _Maxyear=MaxYear;

        }
    }

}