map = L.map('mapid', { editable: true }).setView([54.7370888, 55.9555806], 15);
//привязка событий карты
map.on('click', onMapClick);
map.on('zoomend', onZoomEndMap);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

var USERID = 3;

function onCreateRating() {
    mapStructure.SelectedObject.HasNoRatingYet = false;
    updatePropertyEditor(mapStructure.SelectedObject);
}

function resetPropertyEditor(found) {
    $('#propertyEditor').empty();
}
function updatePropertyEditor(found) {
    var data = found.getObjectSendForm()
    if (found instanceof Sign) {
        $.ajax({
            url: '/Home/GetSignUserEditForm',         /* Куда пойдет запрос */
            method: 'post',             /* Метод передачи (post или get) */
            dataType: 'html',          /* Тип данных в ответе (xml, json, script, html). */
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            async: false,
        }).done(function (gotData) {
            $('#propertyEditor').html(gotData);
        }).fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ', ' + error;
            console.log(err);
        });
    }
    if (found instanceof Maneuver) {
        $.ajax({
            url: '/Home/GetManeuverUserEditForm',         /* Куда пойдет запрос */
            method: 'post',             /* Метод передачи (post или get) */
            dataType: 'html',          /* Тип данных в ответе (xml, json, script, html). */
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            async: false,
        }).done(function (gotData) {
            $('#propertyEditor').html(gotData);
        }).fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ', ' + error;
            console.log(err);
        });
    }
}

function ToggleSelected(found) {
    if (mapStructure.SelectedObject != null) {
        mapStructure.SelectedObject.ResetSelect();
        if (mapStructure.SelectedObject === found) {
            mapStructure.SelectedObject = null;
            resetPropertyEditor(found);
        }
        else {
            mapStructure.SelectedObject = found;
            mapStructure.SelectedObject.SetSelect();
            updatePropertyEditor(found);
        }
    }
    else {
        mapStructure.SelectedObject = found;
        mapStructure.SelectedObject.SetSelect();
        updatePropertyEditor(found);


    }
}

class UserInput {
    constructor(
        userId,
        hasNoRatingYest
    ) {
        this.UserId = userId;
        this.HasNoRatingYet = hasNoRatingYest;
        this.GreenIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.RedIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.BlackIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-black.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.BlueIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
    }
}

function onDifficultyLevelInput() {
    var elements = $("#difficultyLevel");
    var currentValue = Number.parseInt(elements[0].value);
    var min = Number.parseInt(elements[0].min);
    var max = Number.parseInt(elements[0].max);
    if (!isNaN(currentValue)) {
        if (currentValue > max) {
            elements[0].value = elements[0].max;
        }
        if (currentValue < min) {
            elements[0].value = elements[0].min;
        }
        mapStructure.LastSetDifficultyValue = currentValue;
    }
    else {
        elements[0].value = mapStructure.LastSetDifficultyValue;
    }
    mapStructure.AnyChangesToSelectedObject = true;
}
function OnSignPropertyEdit(e) {
    onDifficultyLevelInput();
}
function OnSignRate() {
    if (mapStructure.SelectedObject != null && mapStructure.AnyChangesToSelectedObject) {
        var element = $("#difficultyLevel");
        var t = element.val();
        mapStructure.SelectedObject.DifficultyLevel = Number.parseInt(element.val());
        mapStructure.AnyChangesToSelectedObject = false;
        sendSignDataToChangeRate(mapStructure.SelectedObject.getObjectSendForm());
    }
}
function OnSignClick(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.Marker === e.target;
    });

    switch (mapStructure.MapState) {
        case MapStatesEnum.Nothing:
            ToggleSelected(found);
            break;
        case MapStatesEnum.Deleting:
            if (!mapStructure.ObjectsToRemove.includes(found)) {
                mapStructure.ObjectsToRemove.push(found);
                found.SetSelect();
            }
            else {
                var index = mapStructure.ObjectsToRemove.indexOf(found);
                if (index > -1) {
                    mapStructure.ObjectsToRemove.splice(index, 1);
                }
                found.ResetSelect();
            }
            break;
    }



}
class Sign extends UserInput {
    constructor(
        userId,
        ratedUserId,
        hasNoRatingYest,
        id,
        marker,
        difficultyLevel,
        averageDifficultyLevel,
        signTypeName
    ) {
        super(userId, hasNoRatingYest);
        this.RatedUserId = ratedUserId;
        this.Id = id;
        this.Marker = marker;

        this.Selected = false;
        this.MapStructure = mapStructure;
        this.Marker.setIcon(this.BlueIcon);
        this.DifficultyLevel = difficultyLevel;
        this.AverageDifficultyLevel = averageDifficultyLevel;
        this.SignTypeName = signTypeName;
    }
    ResetSelect() {
        this.Marker.setIcon(this.BlueIcon);
        this.Selected = false;
    }
    SetSelect() {
        this.Marker.setIcon(this.BlackIcon);
        this.Selected = true;
    }

    getObjectSendForm() {
        var point = {
            Lat: this.Marker.getLatLng().lat,
            Lng: this.Marker.getLatLng().lng
        }
        var result = {
            SignId: this.Id,
            MarkerPoint: point,
            DifficultyLevelValue: this.DifficultyLevel,
            AverageDifficultyLevel: this.AverageDifficultyLevel,
            SignTypeName: this.SignTypeName,
            CreatorUserId: this.UserId,
            RatedUserId: this.RatedUserId,
            HasNoRatingYet: this.HasNoRatingYet
        }
        return result;
    }
}

function OnManeuverPropertyEdit(e) {
    onDifficultyLevelInput();
}
function OnManeuverRate() {
    if (mapStructure.SelectedObject != null && mapStructure.AnyChangesToSelectedObject) {
        var element = $("#difficultyLevel");
        mapStructure.SelectedObject.DifficultyLevel = Number.parseInt(element.val());
        
        mapStructure.AnyChangesToSelectedObject = false;
        sendManeuverDataToChangeRate(mapStructure.SelectedObject.getObjectSendForm());
    }
}
function OnManeuverClick(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.StartMarker === e.target || element.EndMarker === e.target;
    });
    switch (mapStructure.MapState) {
        case MapStatesEnum.Nothing:
            ToggleSelected(found);
            break;
        case MapStatesEnum.Deleting:
            if (!mapStructure.ObjectsToRemove.includes(found)) {
                mapStructure.ObjectsToRemove.push(found);
                found.SetSelect();
            }
            else {
                var index = mapStructure.ObjectsToRemove.indexOf(found);
                if (index > -1) {
                    mapStructure.ObjectsToRemove.splice(index, 1);
                }
                found.ResetSelect();
            }
            break;
    }
}
class Maneuver extends UserInput {
    constructor(
        userId,
        ratedUserId,
        hasNoRatingYest,
        id,
        startMarker,
        endMarker,
        polyline,
        difficultyLevel,
        averageDifficultyLevel,
        maneuverTypeName
    ) {
        super(userId, hasNoRatingYest);
        this.RatedUserId = ratedUserId;
        this.Id = id;
        this.StartMarker = startMarker;
        this.EndMarker = endMarker;
        this.Polyline = polyline;

        this.Selected = false;
        this.MapStructure = mapStructure;
        this.StartMarker.setIcon(this.GreenIcon);
        this.EndMarker.setIcon(this.RedIcon);
        this.DifficultyLevel = difficultyLevel;
        this.AverageDifficultyLevel = averageDifficultyLevel;
        this.ManeuverTypeName = maneuverTypeName;
    }
    ResetSelect() {
        this.StartMarker.setIcon(this.GreenIcon);
        this.EndMarker.setIcon(this.RedIcon);
        this.Polyline.deleteArrowheads();
        mapStructure.FiguresOnMap.removeLayer(this.Polyline);
        this.Selected = false;

    }
    SetSelect() {
        this.StartMarker.setIcon(this.BlackIcon);
        this.EndMarker.setIcon(this.BlackIcon);
        this.Polyline.arrowheads();
        mapStructure.FiguresOnMap.addLayer(this.Polyline);
        this.Selected = true;
    }

    getObjectSendForm() {
        var startPoint = {
            Lat: this.StartMarker.getLatLng().lat,
            Lng: this.StartMarker.getLatLng().lng
        }
        var endPoint = {
            Lat: this.EndMarker.getLatLng().lat,
            Lng: this.EndMarker.getLatLng().lng
        }
        var otherPoints = [];
        this.Polyline.getLatLngs().forEach(g => {
            otherPoints.push(
                L.latLng(g.lat, g.lng)
            )
        })
        otherPoints.splice(0, 1);
        otherPoints.splice(otherPoints.length - 1, 1);
        var result = {
            ManeuverId: this.Id,
            StartMarkerPoint: startPoint,
            EndMarkerPoint: endPoint,
            OtherPoints: otherPoints,
            DifficultyLevelValue: this.DifficultyLevel,
            AverageDifficultyLevel: this.AverageDifficultyLevel,
            ManeuverTypeName: this.ManeuverTypeName,
            CreatorUserId: this.UserId,
            RatedUserId: this.RatedUserId,
            HasNoRatingYet: this.HasNoRatingYet
        }
        return result;
    }
}


class MapStructure {
    constructor() {
        this.LastSetDifficultyValue = 1;
        //состояние карты
        this.MapState = MapStatesEnum.Nothing;
        //список всех объектов на карте
        this.ObjectsOnMap = [];
        //их визуализация
        this.FiguresOnMap = L.layerGroup([]).addTo(map);
        this.GreenIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.RedIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.BlackIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-black.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.BlueIcon = new L.Icon({
            iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
            shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowSize: [41, 41]
        });
        this.AddedMarkers = [];
        this.SelectedObject = null;
        this.AnyChangesToSelectedObject = false;
    }
}
const MapStatesEnum = { "Nothing": 0, "Deleting": 1 };
Object.freeze(MapStatesEnum);
var mapStructure = new MapStructure();
LoadSignsFromDB();
LoadManeuversFromDB();

function onMapClick(e) {
}
function onZoomEndMap(e) {
}

function LoadSignsFromDB() {
    $.getJSON({
        url: '/Home/GetOnlyAcceptedSigns?ratedUserId='+USERID,
        async: false
    })
        .done(function (data) {
            data.forEach((sign) => {
               
                var point = L.latLng(sign.MarkerPoint.Lat, sign.MarkerPoint.Lng);
                var marker = L.marker(point, { icon: mapStructure.BlackIcon });
                var signToLoad = new Sign(
                    sign.CreatorUserId,
                    sign.RatedUserId,
                    sign.HasNoRatingYet,
                    sign.SignId,
                    marker,
                    sign.DifficultyLevelValue,
                    sign.AverageDifficultyLevel,
                    sign.SignTypeName);
                mapStructure.FiguresOnMap.addLayer(marker);
                signToLoad.Marker.on('click', OnSignClick);
                mapStructure.ObjectsOnMap.push(signToLoad);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ', ' + error;
        })
}
function LoadManeuversFromDB() {
    $.getJSON({
        url: '/Home/GetOnlyAcceptedManeuvers?ratedUserId='+USERID,
        async: false
    })
        .done(function (data) {
            data.forEach((maneuver) => {
                var startPoint = L.latLng(maneuver.StartMarkerPoint.Lat, maneuver.StartMarkerPoint.Lng);
                var endPoint = L.latLng(maneuver.EndMarkerPoint.Lat, maneuver.EndMarkerPoint.Lng);
                var startMarker = L.marker(startPoint, { icon: mapStructure.BlackIcon });
                var endMarker = L.marker(endPoint, { icon: mapStructure.BlackIcon });
                var coords = [];
                coords.push(startPoint);
                maneuver.OtherPoints.forEach(otherPoint => {
                    coords.push(L.latLng(otherPoint.Lat, otherPoint.Lng));
                })
                coords.push(endPoint);
                var polyline = L.polyline(coords);
                var maneuverToLoad = new Maneuver(
                    maneuver.CreatorUserId,
                    maneuver.RatedUserId,
                    maneuver.HasNoRatingYet,
                    maneuver.ManeuverId,
                    startMarker,
                    endMarker,
                    polyline,
                    maneuver.DifficultyLevelValue,
                    maneuver.AverageDifficultyLevel,
                    maneuver.ManeuverTypeName);
                mapStructure.FiguresOnMap.addLayer(startMarker);
                mapStructure.FiguresOnMap.addLayer(endMarker);
                maneuverToLoad.StartMarker.on('click', OnManeuverClick);
                maneuverToLoad.EndMarker.on('click', OnManeuverClick);
                mapStructure.ObjectsOnMap.push(maneuverToLoad);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ', ' + error;
        })
}



function sendSignDataToChangeRate(signDataToSend) {
    if (signDataToSend == null)
        return;
    $.ajax({
        url: '/Home/ChangeSignsRate',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(signDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
function sendManeuverDataToChangeRate(maneuverDataToSend) {
    if (maneuverDataToSend == null)
        return;
    $.ajax({
        url: '/Home/ChangeManeuversRate',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(maneuverDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
