﻿using Models = Gafarov_VKR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Gafarov_VKR.Controllers
{
    public class HomeController : Controller
    {
        Models.DatabaseContext db = new Models.DatabaseContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Algorithm()
        {
            ViewBag.SignTypes = getSignTypes();
            ViewBag.ManeuverTypes = getManeuverTypes();
            return View();
        }
        [HttpGet]
        public string GetSignTypesForAlgorithm()
        {
            return JsonConvert.SerializeObject(getSignTypes(), Formatting.Indented);
        }
        [HttpGet]
        public string GetManeuverTypesForAlgorithm()
        {
            return JsonConvert.SerializeObject(getManeuverTypes(), Formatting.Indented);
        }


        [HttpPost]
        public PartialViewResult GetSignUserEditForm(Models.MyModels.AcceptedSignModel selectedSign)
        {
            ViewBag.SelectedObject = selectedSign;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetManeuverUserEditForm(Models.MyModels.AcceptedManeuverModel selectedManeuver)
        {
            ViewBag.SelectedObject = selectedManeuver;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetSignApproveForm(Models.MyModels.SignModel selectedSign)
        {
            ViewBag.SignTypes = getSignTypes();
            ViewBag.SelectedObject = selectedSign;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetManeuverApproveForm(Models.MyModels.ManeuverModel selectedManeuver)
        {
            ViewBag.ManeuverTypes = getManeuverTypes();
            ViewBag.SelectedObject = selectedManeuver;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetSignEditForm(Models.MyModels.SignModel selectedSign)
        {
            ViewBag.SignTypes = getSignTypes();
            ViewBag.SelectedObject = selectedSign;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetManeuverEditForm(Models.MyModels.ManeuverModel selectedManeuver)
        {
            ViewBag.ManeuverTypes = getManeuverTypes();
            ViewBag.SelectedObject = selectedManeuver;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetSignShowForm(Models.MyModels.AcceptedSignModel selectedSign)
        {
            ViewBag.SelectedObject = selectedSign;
            return PartialView();
        }
        [HttpPost]
        public PartialViewResult GetManeuverShowtForm(Models.MyModels.AcceptedManeuverModel selectedManeuver)
        {
            ViewBag.SelectedObject = selectedManeuver;
            return PartialView();
        }
        private List<string> getManeuverTypes()
        {
            List<string> types = new List<string>();
            foreach (var type in db.ManeuverTypes)
            {
                types.Add(type.Name);
            }
            return types;
        }
        private List<string> getSignTypes()
        {
            List<string> types = new List<string>();
            foreach (var type in db.SignTypes)
            {
                types.Add(type.Name);
            }
            return types;
        }

        [HttpGet]
        public string GetSignsByUserId(int userId)
        {
            List<Models.MyModels.SignModel> objects = new List<Models.MyModels.SignModel>();

            var userAcceptedSigns = from user in db.Users
                                    join acceptedSign in db.AcceptedSigns on user.Id equals acceptedSign.User_Id
                                    join sign in db.Signs on acceptedSign.Sign_Id equals sign.Id
                                    where user.Id == userId
                                    select sign;

            var userSuggestedSigns = from user in db.Users
                                    join suggestedSign in db.SuggestedSigns on userId equals suggestedSign.User_Id
                                    join sign in db.Signs on suggestedSign.Sign_Id equals sign.Id
                                    where user.Id == userId
                                    select sign;

            var userSigns = new List<Models.Signs>();
            userSigns.AddRange(userAcceptedSigns.ToList());
            userSigns.AddRange(userSuggestedSigns.ToList());
            foreach (var sign in userSigns)
            {
                
                var signType = db.SignTypes.ToList().Find(st => st.Id == sign.SignType_Id);
                //находим ту оценку, что приндалжит этому конкретному пользователю (базовую оценку)
                var signDiffLevel = db.SignDifficlutyLevels.ToList().Find(sdl => sdl.Sign_Id == sign.Id && sdl.User_Id == userId);
                var diffLevel = db.DifficultyLevels.ToList().Find(st => st.Id == signDiffLevel.DifficultyLevel_Id);
                var signMarker = db.SignMarkers.ToList().Find(sm => sm.Sign_Id == sign.Id);
                var marker = db.Markers.ToList().Find(mk => mk.Id == signMarker.Marker_Id);

                bool isEditableSign = true;
                var foundAccepted = db.AcceptedSigns.ToList().Find(t => t.Sign_Id == sign.Id && t.User_Id == userId);
                isEditableSign = foundAccepted != null ?  false :  true;


                objects.Add(new Models.MyModels.SignModel() {
                    Id = sign.Id, 
                    MarkerPoint = new Models.MyModels.PointModel() { Lat = marker.Lat, Lng = marker.Lng},
                    DifficultyLevelValue = diffLevel.Value,
                    SignTypeName = signType.Name,
                    UserId = userId,
                    Editable = isEditableSign
                });
            }

            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }
        [HttpGet]
        public string GetManeuversByUserId(int userId)
        {
            List<Models.MyModels.ManeuverModel> objects = new List<Models.MyModels.ManeuverModel>();


            var userAcceptedManeuvers = from user in db.Users
                                    join acceptedManeuver in db.AcceptedManeuvers on user.Id equals acceptedManeuver.User_Id
                                    join maneuver in db.Maneuvers on acceptedManeuver.Maneuver_Id equals maneuver.Id
                                    where user.Id == userId
                                    select maneuver;

            var userSuggestedManeuvers = from user in db.Users
                                     join suggestedManeuver in db.SuggestedManeuvers on user.Id equals suggestedManeuver.User_Id
                                     join maneuver in db.Maneuvers on suggestedManeuver.Maneuver_Id equals maneuver.Id
                                     where user.Id == userId
                                     select maneuver;

            var userManeuvers = new List<Models.Maneuvers>();
            userManeuvers.AddRange(userAcceptedManeuvers.ToList());
            userManeuvers.AddRange(userSuggestedManeuvers.ToList());

            foreach (var maneuver in userManeuvers)
            {
                var maneuverType = db.ManeuverTypes.ToList().Find(st => st.Id == maneuver.ManeuverType_Id);
                //находим ту оценку, что приндалжит этому конкретному пользователю (базовую оценку)
                var maneuverDiffLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.Maneuver_Id == maneuver.Id && mdl.User_Id == userId);
                var diffLevel = db.DifficultyLevels.ToList().Find(st => st.Id == maneuverDiffLevel.DifficultyLevel_Id);
                var startManeuverMarker = db.StartManeuverMarkers.ToList().Find(smm => smm.Maneuver_Id == maneuver.Id);
                var endManeuverMarker = db.EndManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);
                var startMarker = db.Markers.ToList().Find(mk => mk.Id == startManeuverMarker.Marker_Id);
                var endMarker = db.Markers.ToList().Find(mk => mk.Id == endManeuverMarker.Marker_Id);

                bool isEditableManeuver = true;
                var foundAccepted = db.AcceptedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id && t.User_Id == userId);
                isEditableManeuver = foundAccepted != null ? false : true;


                objects.Add(new Models.MyModels.ManeuverModel()
                {
                    Id = maneuver.Id,
                    StartMarkerPoint = new Models.MyModels.PointModel() { Lat = startMarker.Lat, Lng = startMarker.Lng },
                    EndMarkerPoint = new Models.MyModels.PointModel() { Lat = endMarker.Lat, Lng = endMarker.Lng },
                    DifficultyLevelValue = diffLevel.Value,
                    ManeuverTypeName = maneuverType.Name,
                    UserId = userId,
                    Editable = isEditableManeuver
                });
            }
            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }

        [HttpGet]
        public string GetSigns()
        {

            List<Models.MyModels.SignModel> objects = new List<Models.MyModels.SignModel>();

            foreach (var sign in db.Signs)
            {
                var signType = db.SignTypes.ToList().Find(st => st.Id == sign.SignType_Id);
                var signDiffLevel = db.SignDifficlutyLevels.ToList().Find(sdl => sdl.Sign_Id == sign.Id);
                //подсчёт среднего значения сложности
                var signAllDiffLevels = from dl in db.DifficultyLevels
                                      join sdl in db.SignDifficlutyLevels on dl.Id equals sdl.DifficultyLevel_Id
                                      join s in db.Signs on sdl.Sign_Id equals s.Id
                                      where s.Id == sign.Id
                                      select dl.Value;
                double averageSignDifficultyValue = signAllDiffLevels.Sum() / signAllDiffLevels.Count();
                var signMarker = db.SignMarkers.ToList().Find(sm => sm.Sign_Id == sign.Id);
                var marker = db.Markers.ToList().Find(mk => mk.Id == signMarker.Marker_Id);

                bool isEditableSign = true;
                var foundAccepted = db.AcceptedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                isEditableSign = foundAccepted != null ? false : true;

                int userId = -1;
                if(foundAccepted==null)
                {
                    var foundSuggested = db.SuggestedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                    userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                }
                else
                {
                    userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                }


                objects.Add(new Models.MyModels.SignModel()
                {
                    Id = sign.Id,
                    MarkerPoint = new Models.MyModels.PointModel() { Lat = marker.Lat, Lng = marker.Lng },
                    DifficultyLevelValue = Convert.ToInt32(Math.Floor(averageSignDifficultyValue)),
                    SignTypeName = signType.Name,
                    UserId = userId,
                    Editable = isEditableSign
                });
            }

            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }
        [HttpGet]
        public string GetManeuvers()
        {

            List<Models.MyModels.ManeuverModel> objects = new List<Models.MyModels.ManeuverModel>();

            foreach (var maneuver in db.Maneuvers)
            {
                var maneuverType = db.ManeuverTypes.ToList().Find(st => st.Id == maneuver.ManeuverType_Id);
                var maneuverDiffLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.Maneuver_Id == maneuver.Id);
                //подсчёт среднего значения сложности
                var maneuverAllDiffLevels = from dl in db.DifficultyLevels
                                        join mdl in db.ManeuverDifficlutyLevels on dl.Id equals mdl.DifficultyLevel_Id
                                        join m in db.Maneuvers on mdl.Maneuver_Id equals m.Id
                                        where m.Id == maneuver.Id
                                        select dl.Value;
                double averageManeuverDifficultyValue = maneuverAllDiffLevels.Sum() / maneuverAllDiffLevels.Count();

                var startManeuverMarker = db.StartManeuverMarkers.ToList().Find(smm => smm.Maneuver_Id == maneuver.Id);
                var endManeuverMarker = db.EndManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);
                var startMarker = db.Markers.ToList().Find(mk => mk.Id == startManeuverMarker.Marker_Id);
                var endMarker = db.Markers.ToList().Find(mk => mk.Id == endManeuverMarker.Marker_Id);

                bool isEditableManeuver = true;
                var foundAccepted = db.AcceptedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                isEditableManeuver = foundAccepted != null ? false : true;

                int userId = -1;
                if (foundAccepted == null)
                {
                    var foundSuggested = db.SuggestedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                    userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                }
                else
                {
                    userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                }


                objects.Add(new Models.MyModels.ManeuverModel()
                {
                    Id = maneuver.Id,
                    StartMarkerPoint = new Models.MyModels.PointModel() { Lat = startMarker.Lat, Lng = startMarker.Lng },
                    EndMarkerPoint = new Models.MyModels.PointModel() { Lat = endMarker.Lat, Lng = endMarker.Lng },
                    DifficultyLevelValue = Convert.ToInt32(Math.Floor(averageManeuverDifficultyValue)),
                    ManeuverTypeName = maneuverType.Name,
                    UserId = userId,
                    Editable = isEditableManeuver
                });
            }
            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }

        [HttpGet]
        public string GetOnlyAcceptedSigns(int ratedUserId)
        {
            List<Models.MyModels.AcceptedSignModel> objects = new List<Models.MyModels.AcceptedSignModel>();

            foreach (var sign in db.Signs)
            {
                bool isEditableSign = true;
                var foundAccepted = db.AcceptedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                isEditableSign = foundAccepted != null ? false : true;
                if(!isEditableSign)
                {
                    var signType = db.SignTypes.ToList().Find(st => st.Id == sign.SignType_Id);
                    //подсчёт среднего значения сложности
                    var signAllDiffLevels = from dl in db.DifficultyLevels
                                            join sdl in db.SignDifficlutyLevels on dl.Id equals sdl.DifficultyLevel_Id
                                            join s in db.Signs on sdl.Sign_Id equals s.Id
                                            where s.Id == sign.Id
                                            select dl.Value;
                    double averageSignDifficultyValue = signAllDiffLevels.Sum() / signAllDiffLevels.Count();
                    var signMarker = db.SignMarkers.ToList().Find(sm => sm.Sign_Id == sign.Id);
                    var marker = db.Markers.ToList().Find(mk => mk.Id == signMarker.Marker_Id);

                    //находим создателя
                    int userId = -1;
                    if (foundAccepted == null)
                    {
                        var foundSuggested = db.SuggestedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                        userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                    }
                    else
                    {
                        userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                    }

                    //тут ищем оценку пользователя ratedUserId (проголосовавшего пользователя)
                    bool hasHasNoRatingYet = true;
                    int userDifficultyRate = 1;
                    var signDifficultyLevel = db.SignDifficlutyLevels.ToList().Find(sdl => sdl.User_Id == ratedUserId && sdl.Sign_Id == sign.Id);
                    if(signDifficultyLevel!=null)
                    {
                        var diffLevel = db.DifficultyLevels.ToList().Find(dl => dl.Id == signDifficultyLevel.DifficultyLevel_Id);
                        userDifficultyRate = diffLevel.Value;
                        hasHasNoRatingYet = false;
                    }

                    objects.Add(new Models.MyModels.AcceptedSignModel()
                    {
                        SignId = sign.Id,
                        MarkerPoint = new Models.MyModels.PointModel() { Lat = marker.Lat, Lng = marker.Lng },
                        DifficultyLevelValue = userDifficultyRate,
                        AverageDifficultyLevel = Convert.ToInt32(Math.Floor(averageSignDifficultyValue)),
                        SignTypeName = signType.Name,
                        CreatorUserId = userId,
                        RatedUserId = ratedUserId,
                        HasNoRatingYet = hasHasNoRatingYet
                    });
                }
            }

            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }
        [HttpGet]
        public string GetOnlyAcceptedManeuvers(int ratedUserId)
        {
            List<Models.MyModels.AcceptedManeuverModel> objects = new List<Models.MyModels.AcceptedManeuverModel>();

            foreach (var maneuver in db.Maneuvers)
            {
                bool isEditableManeuver = true;
                var foundAccepted = db.AcceptedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                isEditableManeuver = foundAccepted != null ? false : true;
                
                if(!isEditableManeuver)
                {
                    var maneuverType = db.ManeuverTypes.ToList().Find(st => st.Id == maneuver.ManeuverType_Id);
                    var maneuverDiffLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.Maneuver_Id == maneuver.Id);
                    //подсчёт среднего значения сложности
                    var maneuverAllDiffLevels = from dl in db.DifficultyLevels
                                                join mdl in db.ManeuverDifficlutyLevels on dl.Id equals mdl.DifficultyLevel_Id
                                                join m in db.Maneuvers on mdl.Maneuver_Id equals m.Id
                                                where m.Id == maneuver.Id
                                                select dl.Value;
                    double averageManeuverDifficultyValue = maneuverAllDiffLevels.Sum() / maneuverAllDiffLevels.Count();

                    var startManeuverMarker = db.StartManeuverMarkers.ToList().Find(smm => smm.Maneuver_Id == maneuver.Id);
                    var endManeuverMarker = db.EndManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);
                    var startMarker = db.Markers.ToList().Find(mk => mk.Id == startManeuverMarker.Marker_Id);
                    var endMarker = db.Markers.ToList().Find(mk => mk.Id == endManeuverMarker.Marker_Id);


                    //находим создателя
                    int userId = -1;
                    if (foundAccepted == null)
                    {
                        var foundSuggested = db.SuggestedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                        userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                    }
                    else
                    {
                        userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                    }

                    //тут ищем оценку пользователя ratedUserId (проголосовавшего пользователя)
                    bool hasNoRatingYet = true;
                    int userDifficultyRate = 1;
                    var maneuverDifficultyLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.User_Id == ratedUserId && mdl.Maneuver_Id == maneuver.Id);
                    if (maneuverDifficultyLevel != null)
                    {
                        var diffLevel = db.DifficultyLevels.ToList().Find(dl => dl.Id == maneuverDifficultyLevel.DifficultyLevel_Id);
                        userDifficultyRate = diffLevel.Value;
                        hasNoRatingYet = false;
                    }

                    objects.Add(new Models.MyModels.AcceptedManeuverModel()
                    {
                        ManeuverId = maneuver.Id,
                        StartMarkerPoint = new Models.MyModels.PointModel() { Lat = startMarker.Lat, Lng = startMarker.Lng },
                        EndMarkerPoint = new Models.MyModels.PointModel() { Lat = endMarker.Lat, Lng = endMarker.Lng },
                        DifficultyLevelValue = userDifficultyRate,
                        AverageDifficultyLevel = Convert.ToInt32(Math.Floor(averageManeuverDifficultyValue)),
                        ManeuverTypeName = maneuverType.Name,
                        CreatorUserId = userId,
                        RatedUserId = ratedUserId,
                        HasNoRatingYet = hasNoRatingYet
                    });
                }
            }
            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }

        //добавление объектов
        [HttpPost]
        public void AddSigns(List<Models.MyModels.SignModel> dataOfSigns)
        {
            foreach (var dataOfSign in dataOfSigns)
            {
                var difficultyLevel = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfSign.DifficultyLevelValue);
                var signType = db.SignTypes.ToList().Find(st => st.Name == dataOfSign.SignTypeName);
                
                Models.Signs sign = new Models.Signs() { SignType_Id = signType.Id};
                db.Signs.Add(sign);
                Models.Markers marker = new Models.Markers() { Lat = dataOfSign.MarkerPoint.Lat, Lng = dataOfSign.MarkerPoint.Lng };
                db.Markers.Add(marker);
                db.SaveChanges();

                Models.SignDifficlutyLevels signDifficultyLevel = new Models.SignDifficlutyLevels()
                {
                    User_Id = dataOfSign.UserId,
                    Sign_Id = sign.Id,
                    DifficultyLevel_Id = difficultyLevel.Id
                };
                db.SignDifficlutyLevels.Add(signDifficultyLevel);

                Models.SignMarkers signMarker = new Models.SignMarkers() { Sign_Id = sign.Id, Marker_Id = marker.Id };
                db.SignMarkers.Add(signMarker);

                Models.SuggestedSigns suggestedSign = new Models.SuggestedSigns() { Sign_Id = sign.Id, User_Id = dataOfSign.UserId };
                db.SuggestedSigns.Add(suggestedSign);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void AddManeuvers(List<Models.MyModels.ManeuverModel> dataOfManeuvers)
        {
            foreach (var dataOfManeuver in dataOfManeuvers)
            {
                var difficultyLevel = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfManeuver.DifficultyLevelValue);
                
                var maneuverType = db.ManeuverTypes.ToList().Find(st => st.Name == dataOfManeuver.ManeuverTypeName);
                
                Models.Maneuvers maneuver = new Models.Maneuvers() { ManeuverType_Id = maneuverType.Id };
                Models.Markers startMarker = new Models.Markers() { Lat = dataOfManeuver.StartMarkerPoint.Lat, Lng = dataOfManeuver.StartMarkerPoint.Lng };
                Models.Markers endMarker = new Models.Markers() { Lat = dataOfManeuver.EndMarkerPoint.Lat, Lng = dataOfManeuver.EndMarkerPoint.Lng };
                db.Markers.Add(startMarker);
                db.Markers.Add(endMarker);
                db.Maneuvers.Add(maneuver);
                db.SaveChanges();

                Models.ManeuverDifficlutyLevels maneuverDifficultyLevel = new Models.ManeuverDifficlutyLevels()
                {
                    User_Id = dataOfManeuver.UserId,
                    Maneuver_Id = maneuver.Id,
                    DifficultyLevel_Id = difficultyLevel.Id
                };
                db.ManeuverDifficlutyLevels.Add(maneuverDifficultyLevel);

                Models.StartManeuverMarkers startManeuverMarker = new Models.StartManeuverMarkers() { Maneuver_Id = maneuver.Id, Marker_Id = startMarker.Id };
                db.StartManeuverMarkers.Add(startManeuverMarker);
                Models.EndManeuverMarkers endManeuverMarker = new Models.EndManeuverMarkers() { Maneuver_Id = maneuver.Id, Marker_Id = endMarker.Id };
                db.EndManeuverMarkers.Add(endManeuverMarker);

                Models.SuggestedManeuvers suggestedManeuver = new Models.SuggestedManeuvers() { Maneuver_Id = maneuver.Id, User_Id = dataOfManeuver.UserId };
                db.SuggestedManeuvers.Add(suggestedManeuver);

                db.SaveChanges();
            }
        }
        [HttpPost]
        public void AddUsers(List<Models.MyModels.UserModel> dataOfUsers)
        {
            foreach(var dataOfUser in dataOfUsers)
            {
                db.Users.Add(new Models.Users() { Login = dataOfUser.Login, Password = dataOfUser.Password });
            }
        }


        private void removeSign(Models.Signs signToDelete)
        {
            var markersToDelete = from marker in db.Markers
                                  join signMarker in db.SignMarkers on marker.Id equals signMarker.Marker_Id
                                  join sign in db.Signs on signMarker.Sign_Id equals sign.Id
                                  where sign.Id == signToDelete.Id
                                  select marker;
            db.Markers.RemoveRange(markersToDelete);
            db.Signs.Remove(signToDelete);
            db.SaveChanges();
        }
        [HttpPost]
        public void RemoveSigns(List<Models.MyModels.SignModel> signs)
        {
            foreach (var obj in signs)
            {
                var signToDelete = db.Signs.ToList().Find(s => s.Id == obj.Id);
                removeSign(signToDelete);
            }
        }

        private void removeManeuver(Models.Maneuvers maneuverToDelete)
        {
            var startMarkersToDelete = from marker in db.Markers
                                       join startManeuverMarker in db.StartManeuverMarkers on marker.Id equals startManeuverMarker.Marker_Id
                                       join maneuver in db.Maneuvers on startManeuverMarker.Maneuver_Id equals maneuver.Id
                                       where maneuver.Id == maneuverToDelete.Id
                                       select marker;

            var endMarkersToDelete = from marker in db.Markers
                                     join endManeuverMarker in db.EndManeuverMarkers on marker.Id equals endManeuverMarker.Marker_Id
                                     join maneuver in db.Maneuvers on endManeuverMarker.Maneuver_Id equals maneuver.Id
                                     where maneuver.Id == maneuverToDelete.Id
                                     select marker;

            db.Markers.RemoveRange(startMarkersToDelete);
            db.Markers.RemoveRange(endMarkersToDelete);
            db.Maneuvers.Remove(maneuverToDelete);
            db.SaveChanges();
        }
        [HttpPost]
        public void RemoveManeuvers(List<Models.MyModels.ManeuverModel> maneuvers)
        {
            foreach (var obj in maneuvers)
            {
                var maneuverToDelete = db.Maneuvers.ToList().Find(m => m.Id == obj.Id);
                removeManeuver(maneuverToDelete);
            }
            
        }

        private void removeUser(Models.Users userToDelete)
        {
            var acceptedManeuversToDelete = from maneuver in db.Maneuvers
                                    join acceptedManeuver in db.AcceptedManeuvers on maneuver.Id equals acceptedManeuver.Maneuver_Id
                                    join user in db.Users on acceptedManeuver.User_Id equals user.Id
                                    where user.Id == userToDelete.Id
                                    select maneuver;
            var acceptedSignsToDelete = from sign in db.Signs
                                join acceptedSign in db.AcceptedSigns on sign.Id equals acceptedSign.Sign_Id
                                join user in db.Users on acceptedSign.User_Id equals user.Id
                                where user.Id == userToDelete.Id
                                select sign;
            foreach(var maneuverToDelete in acceptedManeuversToDelete)
            {
                removeManeuver(maneuverToDelete);
            }
            foreach (var signToDelete in acceptedSignsToDelete)
            {
                removeSign(signToDelete);
            }

            var suggestedManeuversToDelete = from maneuver in db.Maneuvers
                                            join suggestedManeuver in db.SuggestedManeuvers on maneuver.Id equals suggestedManeuver.Maneuver_Id
                                            join user in db.Users on suggestedManeuver.User_Id equals user.Id
                                            where user.Id == userToDelete.Id
                                            select maneuver;
            var suggestedSignsToDelete = from sign in db.Signs
                                        join suggestedSign in db.SuggestedSigns on sign.Id equals suggestedSign.Sign_Id
                                        join user in db.Users on suggestedSign.User_Id equals user.Id
                                        where user.Id == userToDelete.Id
                                        select sign;
            foreach (var maneuverToDelete in suggestedManeuversToDelete)
            {
                removeManeuver(maneuverToDelete);
            }
            foreach (var signToDelete in suggestedSignsToDelete)
            {
                removeSign(signToDelete);
            }

            db.Users.Remove(userToDelete);
            db.SaveChanges();

        }
        [HttpPost]
        public void RemoveUsers(List<Models.MyModels.ManeuverModel> users)
        {
            foreach (var obj in users)
            {
                var userToDelete = db.Users.ToList().Find(u => u.Id == obj.Id);
                removeUser(userToDelete);
            }

        }


        [HttpPost]
        public void ChangeSigns(List<Models.MyModels.SignModel> dataOfSigns)
        {
            foreach(var dataOfSign in dataOfSigns)
            {
                var sign = db.Signs.ToList().Find(s => s.Id == dataOfSign.Id);
                var signMarker = db.SignMarkers.ToList().Find(sm => sm.Sign_Id == sign.Id);
                var marker = db.Markers.ToList().Find(m => m.Id == signMarker.Marker_Id);
                //обновление значений координат
                marker.Lat = dataOfSign.MarkerPoint.Lat;
                marker.Lng = dataOfSign.MarkerPoint.Lng;

                //обновление значений уровня слонжности и типа знака
                var signDifficultyLevel = db.SignDifficlutyLevels.ToList().Find(sdl => sdl.Sign_Id == sign.Id && sdl.User_Id == dataOfSign.UserId);
                if(signDifficultyLevel!=null)
                {
                    var difficultyLevelToSet_Id = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfSign.DifficultyLevelValue).Id;
                    signDifficultyLevel.DifficultyLevel_Id = difficultyLevelToSet_Id;
                }

                var signTypeToSet_Id = db.SignTypes.ToList().Find(st => st.Name == dataOfSign.SignTypeName).Id;
                sign.SignType_Id = signTypeToSet_Id;
            }
            db.SaveChanges();
        }
        [HttpPost]
        public void ChangeManeuvers(List<Models.MyModels.ManeuverModel> dataOfManeuvers)
        {
            foreach (var dataOfManeuver in dataOfManeuvers)
            {
                var maneuver = db.Maneuvers.ToList().Find(man => man.Id == dataOfManeuver.Id);
                var endManeuverMarker = db.EndManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);
                var startManeuverMarker = db.StartManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);

                var startMarker = db.Markers.ToList().Find(m => m.Id == startManeuverMarker.Marker_Id);
                startMarker.Lat = dataOfManeuver.StartMarkerPoint.Lat;
                startMarker.Lng = dataOfManeuver.StartMarkerPoint.Lng;
                var endMarker = db.Markers.ToList().Find(m => m.Id == endManeuverMarker.Marker_Id);
                endMarker.Lat = dataOfManeuver.EndMarkerPoint.Lat;
                endMarker.Lng = dataOfManeuver.EndMarkerPoint.Lng;

                //обновление значений уровня слонжности и типа манёвра
                var maneuverDifficultyLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.Maneuver_Id == maneuver.Id && mdl.User_Id == dataOfManeuver.UserId);
                if(maneuverDifficultyLevel!=null)
                {
                    var difficultyLevelToSet_Id = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfManeuver.DifficultyLevelValue).Id;
                    maneuverDifficultyLevel.DifficultyLevel_Id = difficultyLevelToSet_Id;
                }

                var maneuverTypeToSet_Id = db.ManeuverTypes.ToList().Find(st => st.Name == dataOfManeuver.ManeuverTypeName).Id;
                maneuver.ManeuverType_Id = maneuverTypeToSet_Id;
            }
            db.SaveChanges();
        }


        [HttpPost]
        public void ApproveSigns(List<Models.MyModels.SignModel> dataOfSigns)
        {
            foreach(var dataOfSign in dataOfSigns)
            {
                if((db.AcceptedSigns.Where(acceptedSign=>acceptedSign.Sign_Id == dataOfSign.Id)).Count() == 0)
                {
                    var suggestedSign = db.SuggestedSigns.ToList().Find(suggSign => suggSign.Sign_Id == dataOfSign.Id);
                    db.SuggestedSigns.Remove(suggestedSign);
                    db.AcceptedSigns.Add(new Models.AcceptedSigns() { Sign_Id = dataOfSign.Id, User_Id = dataOfSign.UserId });
                }
            }
            db.SaveChanges();
        }
        [HttpPost]
        public void ApproveManeuvers(List<Models.MyModels.ManeuverModel> dataOfManeuvers)
        {
            foreach (var dataOfManeuver in dataOfManeuvers)
            {
                if ((db.AcceptedManeuvers.Where(acceptedManeuver => acceptedManeuver.Maneuver_Id == dataOfManeuver.Id)).Count() == 0)
                {
                    var suggestedManeuver = db.SuggestedManeuvers.ToList().Find(suggManeuver => suggManeuver.Maneuver_Id == dataOfManeuver.Id);
                    db.SuggestedManeuvers.Remove(suggestedManeuver);
                    db.AcceptedManeuvers.Add(new Models.AcceptedManeuvers() { Maneuver_Id = dataOfManeuver.Id, User_Id = dataOfManeuver.UserId });
                }
            }
            db.SaveChanges();
        }

        [HttpPost]
        public void DisapproveSigns(List<Models.MyModels.SignModel> dataOfSigns)
        {
            foreach (var dataOfSign in dataOfSigns)
            {
                if ((db.AcceptedSigns.Where(acceptedSign => acceptedSign.Sign_Id == dataOfSign.Id)).Count() > 0)
                {
                    var acceptedSign = db.AcceptedSigns.ToList().Find(accSign => accSign.Sign_Id == dataOfSign.Id);
                    db.AcceptedSigns.Remove(acceptedSign);
                    db.SuggestedSigns.Add(new Models.SuggestedSigns() { Sign_Id = dataOfSign.Id, User_Id = dataOfSign.UserId });
                }
            }
            db.SaveChanges();
        }
        [HttpPost]
        public void DisapproveManeuvers(List<Models.MyModels.ManeuverModel> dataOfManeuvers)
        {
            foreach (var dataOfManeuver in dataOfManeuvers)
            {
                if ((db.AcceptedManeuvers.Where(acceptedManeuver => acceptedManeuver.Maneuver_Id == dataOfManeuver.Id)).Count() > 0)
                {
                    var acceptedManeuver = db.AcceptedManeuvers.ToList().Find(accManeuver => accManeuver.Maneuver_Id == dataOfManeuver.Id);
                    db.AcceptedManeuvers.Remove(acceptedManeuver);
                    db.SuggestedManeuvers.Add(new Models.SuggestedManeuvers() { Maneuver_Id = dataOfManeuver.Id, User_Id = dataOfManeuver.UserId });
                }
            }
            db.SaveChanges();
        }

        [HttpPost]
        public void ChangeSignsRate(Models.MyModels.AcceptedSignModel dataOfSign)
        {
            var difficultyLevelToSet_Id = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfSign.DifficultyLevelValue).Id;
            var signDifficultyLevel = db.SignDifficlutyLevels.ToList().Find(sdl => sdl.User_Id == dataOfSign.RatedUserId && sdl.Sign_Id == dataOfSign.SignId);
            if(signDifficultyLevel!=null)
            {
                signDifficultyLevel.DifficultyLevel_Id = difficultyLevelToSet_Id;
            }
            else
            {
                db.SignDifficlutyLevels.Add(new Models.SignDifficlutyLevels()
                {
                    User_Id = dataOfSign.RatedUserId,
                    Sign_Id = dataOfSign.SignId,
                    DifficultyLevel_Id = difficultyLevelToSet_Id
                });
            }
            db.SaveChanges();
        }
        [HttpPost]
        public void ChangeManeuversRate(Models.MyModels.AcceptedManeuverModel dataOfManeuver)
        {
            var difficultyLevelToSet_Id = db.DifficultyLevels.ToList().Find(dl => dl.Value == dataOfManeuver.DifficultyLevelValue).Id;
            var maneuverDifficlutyLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.User_Id == dataOfManeuver.RatedUserId && mdl.Maneuver_Id == dataOfManeuver.ManeuverId);
            if (maneuverDifficlutyLevel != null)
            {
                maneuverDifficlutyLevel.DifficultyLevel_Id = difficultyLevelToSet_Id;
            }
            else
            {
                db.ManeuverDifficlutyLevels.Add(new Models.ManeuverDifficlutyLevels()
                {
                    User_Id = dataOfManeuver.RatedUserId,
                    Maneuver_Id = dataOfManeuver.ManeuverId,
                    DifficultyLevel_Id = difficultyLevelToSet_Id
                });
            }
            db.SaveChanges();
        }

        [HttpGet]
        public string GetSignsForAlgorithm()
        {
            List<Models.MyModels.AcceptedSignModel> objects = new List<Models.MyModels.AcceptedSignModel>();

            foreach (var sign in db.Signs)
            {
                bool isEditableSign = true;
                var foundAccepted = db.AcceptedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                isEditableSign = foundAccepted != null ? false : true;
                if (!isEditableSign)
                {
                    var signType = db.SignTypes.ToList().Find(st => st.Id == sign.SignType_Id);
                    //подсчёт среднего значения сложности
                    var signAllDiffLevels = from dl in db.DifficultyLevels
                                            join sdl in db.SignDifficlutyLevels on dl.Id equals sdl.DifficultyLevel_Id
                                            join s in db.Signs on sdl.Sign_Id equals s.Id
                                            where s.Id == sign.Id
                                            select dl.Value;
                    double averageSignDifficultyValue = signAllDiffLevels.Sum() / signAllDiffLevels.Count();
                    var signMarker = db.SignMarkers.ToList().Find(sm => sm.Sign_Id == sign.Id);
                    var marker = db.Markers.ToList().Find(mk => mk.Id == signMarker.Marker_Id);

                    //находим создателя
                    int userId = -1;
                    if (foundAccepted == null)
                    {
                        var foundSuggested = db.SuggestedSigns.ToList().Find(t => t.Sign_Id == sign.Id);
                        userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                    }
                    else
                    {
                        userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                    }

                    objects.Add(new Models.MyModels.AcceptedSignModel()
                    {
                        SignId = sign.Id,
                        MarkerPoint = new Models.MyModels.PointModel() { Lat = marker.Lat, Lng = marker.Lng },
                        DifficultyLevelValue = -1,
                        AverageDifficultyLevel = Convert.ToInt32(Math.Floor(averageSignDifficultyValue)),
                        SignTypeName = signType.Name,
                        CreatorUserId = userId,
                        RatedUserId = -1,
                        HasNoRatingYet = false
                    });
                }
            }

            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }
        [HttpGet]
        public string GetManeuversForAlgorithm()
        {
            List<Models.MyModels.AcceptedManeuverModel> objects = new List<Models.MyModels.AcceptedManeuverModel>();

            foreach (var maneuver in db.Maneuvers)
            {
                bool isEditableManeuver = true;
                var foundAccepted = db.AcceptedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                isEditableManeuver = foundAccepted != null ? false : true;

                if (!isEditableManeuver)
                {
                    var maneuverType = db.ManeuverTypes.ToList().Find(st => st.Id == maneuver.ManeuverType_Id);
                    var maneuverDiffLevel = db.ManeuverDifficlutyLevels.ToList().Find(mdl => mdl.Maneuver_Id == maneuver.Id);
                    //подсчёт среднего значения сложности
                    var maneuverAllDiffLevels = from dl in db.DifficultyLevels
                                                join mdl in db.ManeuverDifficlutyLevels on dl.Id equals mdl.DifficultyLevel_Id
                                                join m in db.Maneuvers on mdl.Maneuver_Id equals m.Id
                                                where m.Id == maneuver.Id
                                                select dl.Value;
                    double averageManeuverDifficultyValue = maneuverAllDiffLevels.Sum() / maneuverAllDiffLevels.Count();

                    var startManeuverMarker = db.StartManeuverMarkers.ToList().Find(smm => smm.Maneuver_Id == maneuver.Id);
                    var endManeuverMarker = db.EndManeuverMarkers.ToList().Find(emm => emm.Maneuver_Id == maneuver.Id);
                    var startMarker = db.Markers.ToList().Find(mk => mk.Id == startManeuverMarker.Marker_Id);
                    var endMarker = db.Markers.ToList().Find(mk => mk.Id == endManeuverMarker.Marker_Id);


                    //находим создателя
                    int userId = -1;
                    if (foundAccepted == null)
                    {
                        var foundSuggested = db.SuggestedManeuvers.ToList().Find(t => t.Maneuver_Id == maneuver.Id);
                        userId = db.Users.ToList().Find(us => us.Id == foundSuggested.User_Id).Id;
                    }
                    else
                    {
                        userId = db.Users.ToList().Find(us => us.Id == foundAccepted.User_Id).Id;
                    }


                    objects.Add(new Models.MyModels.AcceptedManeuverModel()
                    {
                        ManeuverId = maneuver.Id,
                        StartMarkerPoint = new Models.MyModels.PointModel() { Lat = startMarker.Lat, Lng = startMarker.Lng },
                        EndMarkerPoint = new Models.MyModels.PointModel() { Lat = endMarker.Lat, Lng = endMarker.Lng },
                        DifficultyLevelValue = -1,
                        AverageDifficultyLevel = Convert.ToInt32(Math.Floor(averageManeuverDifficultyValue)),
                        ManeuverTypeName = maneuverType.Name,
                        CreatorUserId = userId,
                        RatedUserId = -1,
                        HasNoRatingYet = false
                    });
                }
            }
            return JsonConvert.SerializeObject(objects, Formatting.Indented);
        }

        [HttpPost]
        public void StartAlgorithm(Models.MyModels.UserDataModel userData)
        {
            Dictionary<string, int> signProblems = new Dictionary<string, int>();
            foreach(var list in userData.SignProblems)
            {
                signProblems.Add((string)list[0], (int)list[1]);
            }
            Dictionary<string, int> maneuverProblems = new Dictionary<string, int>();
            foreach (var list in userData.ManeuverProblems)
            {
                maneuverProblems.Add((string)list[0], (int)list[1]);
            }
        }

    }
}