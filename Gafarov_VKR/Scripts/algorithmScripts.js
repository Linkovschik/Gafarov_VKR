map = L.map('mapid', { editable: true }).setView([54.7370888, 55.9555806], 15);
//привязка событий карты
map.on('click', onMapClick);
map.on('zoomend', onZoomEndMap);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);


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
            url: '/Home/GetSignShowForm',         /* Куда пойдет запрос */
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
            url: '/Home/GetManeuverShowtForm',         /* Куда пойдет запрос */
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

function onSignProblemPoleBlur(elementId) {
    var index = elementId.indexOf('_');
    if (index > -1) {
        var keyIndex = Number.parseInt(elementId.substring(index + 1));
        if (isNaN(keyIndex)) {
            return;
        }
        var element = $("#" + elementId);
        element[0].value = userData.getSignTypeValue(keyIndex);
    }
}
function onManeuverProblemPoleBlur(elementId) {
    var index = elementId.indexOf('_');
    if (index > -1) {
        var keyIndex = Number.parseInt(elementId.substring(index + 1));
        if (isNaN(keyIndex)) {
            return;
        }
        var element = $("#" + elementId);
        element[0].value = userData.getManeuverTypeValue(keyIndex);
    }
}
function OnSignPropertyEdit(elementId) {
    var maxValue = 2147483646;
    var index = elementId.indexOf('_');
    if (index > -1) {
        var keyIndex = Number.parseInt(elementId.substring(index+1));
        if (isNaN(keyIndex)) {
            return;
        }
        var element = $("#" + elementId);
        var value = Number.parseInt(element.val());
        if (!isNaN(value) && value >= 0 && value < maxValue) {
            userData.setSignTypeValue(keyIndex, value);
        }
    }
}
function OnManeuverPropertyEdit(elementId) {
    var maxValue = 2147483646;
    var index = elementId.indexOf('_');
    if (index > -1) {
        var keyIndex = Number.parseInt(elementId.substring(index+1));
        if (isNaN(keyIndex)) {
            return;
        }
        var element = $("#" + elementId);
        var value = Number.parseInt(element.val());
        if (!isNaN(value) && value >= 0 && value < maxValue) {
            userData.setManeuverTypeValue(keyIndex, value);
        }
    }
}

function OnSignClick(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.Marker === e.target;
    });

    if (mapStructure.MapState == MapStatesEnum.Nothing) {
        ToggleSelected(found);
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
        this.StartMarker = null;
        this.APIControl = null;
    }
}

class UserData {
    constructor() {
        this.StartPoint = {
            Lat: -1.0,
            Lng: -1.0
        }
        this.SignProblemMap = new Map();
        this.SignIndexMap = new Map();
        this.ManeuverProblemMap = new Map();
        this.ManeuverIndexMap = new Map();
    }

    setSignTypes(signTypes) {
        signTypes.forEach(signType => {
            this.SignProblemMap.set(signType, 1);
            var index = this.SignIndexMap.size;
            this.SignIndexMap.set(index, signType);
        });
    }
    setSignTypeValue(keyIndex, keyValue) {
        if (keyIndex >= 0 && keyIndex < this.SignIndexMap.size) {
            var keySignType = this.SignIndexMap.get(keyIndex);
            this.SignProblemMap.set(keySignType, keyValue);
        }
    }
    getSignTypeValue(keyIndex) {
        if (keyIndex >= 0 && keyIndex < this.SignIndexMap.size) {
            var keySignType = this.SignIndexMap.get(keyIndex);
            return this.SignProblemMap.get(keySignType);
        }
        else {
            return 0
        }
    }
    setManeuverTypes(maneuverTypes) {
        maneuverTypes.forEach(maneuverType => {
            this.ManeuverProblemMap.set(maneuverType, 1);
            var index = this.ManeuverIndexMap.size;
            this.ManeuverIndexMap.set(index, maneuverType);
        });
    }
    setManeuverTypeValue(keyIndex, keyValue) {
        if (keyIndex >= 0 && keyIndex < this.SignIndexMap.size) {
            var keyManeuverType = this.ManeuverIndexMap.get(keyIndex);
            this.ManeuverProblemMap.set(keyManeuverType, keyValue);
        }
    }
    getManeuverTypeValue(keyIndex) {
        if (keyIndex >= 0 && keyIndex < this.SignIndexMap.size) {
            var keySignType = this.ManeuverIndexMap.get(keyIndex);
            return this.ManeuverProblemMap.get(keySignType);
        }
        else {
            return 0
        }
    }
    setStartPointCoordinates(lat, lng) {
        this.StartPoint.Lat = lat;
        this.StartPoint.Lng = lng;
    }
}

const MapStatesEnum = { "Nothing": 0, "SetStart": 1 };
Object.freeze(MapStatesEnum);
var mapStructure = new MapStructure();
LoadSignsFromDB();
LoadManeuversFromDB();
var userData = new UserData();
LoadSignTypesFromDB();
LoadManeuverTypesFromDB();


function onMapClick(e) {
    if (mapStructure.MapState == MapStatesEnum.SetStart) {
        if (mapStructure.StartMarker != null) {
            mapStructure.FiguresOnMap.removeLayer(mapStructure.StartMarker);
        }
        mapStructure.StartMarker = L.marker(e.latlng, { icon: mapStructure.BlackIcon });
        userData.setStartPointCoordinates(e.latlng.lat, e.latlng.lng);
        mapStructure.FiguresOnMap.addLayer(mapStructure.StartMarker);
        readyForAlgorithm();
    }
}
function onZoomEndMap(e) {
}

function LoadSignsFromDB() {
    $.getJSON({
        url: '/Home/GetSignsForAlgorithm',
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
        url: '/Home/GetManeuversForAlgorithm',
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
function LoadSignTypesFromDB() {
    $.getJSON({
        url: '/Home/GetSignTypesForAlgorithm',
        async: false
    })
    .done(function (data) {
        var signTypes = [];
        data.forEach((signType) => {
            signTypes.push(signType);
        });
        userData.setSignTypes(signTypes);
    })
    .fail(function (jqxhr, textStatus, error) {
        var err = textStatus + ', ' + error;
    })
} 
function LoadManeuverTypesFromDB() {
    $.getJSON({
        url: '/Home/GetManeuverTypesForAlgorithm',
        async: false
    })
    .done(function (data) {
        var maneuverTypes = [];
        data.forEach((maneuverType) => {
            maneuverTypes.push(maneuverType);
        });
        userData.setManeuverTypes(maneuverTypes);
    })
    .fail(function (jqxhr, textStatus, error) {
        var err = textStatus + ', ' + error;
    })
}


function setStartPosition() {
    if (mapStructure.SelectedObject != null) {
        ToggleSelected(mapStructure.SelectedObject)
    }
    mapStructure.MapState = MapStatesEnum.SetStart;
    $("#cancelBuildButton").removeClass('hide');
}
function cancelPosition() {
    mapStructure.MapState = MapStatesEnum.Nothing;
    $("#cancelBuildButton").addClass('hide');
}
function readyForAlgorithm() {
    mapStructure.FiguresOnMap.clearLayers();
    mapStructure.FiguresOnMap.addLayer(mapStructure.StartMarker);
    if (mapStructure.APIControl != null)
        map.removeControl(mapStructure.APIControl);
    $('#algorithmButton').removeAttr('disabled');
    cancelPosition();
}

function startAlgorithm() {
    readyForAlgorithm();
    if (userData.StartPoint.Lat == -1 || userData.StartPoint.Lng == -1)
        return;
    var result_waypoints = [];
    var userDataToSend = {
        SignProblems: [...userData.SignProblemMap],
        ManeuverProblems: [...userData.ManeuverProblemMap],
        StartPosition: userData.StartPoint
    }
    var data = JSON.stringify(userDataToSend);
    $.ajax({
        url: '/Home/StartAlgorithm',         
        method: 'post',            
        dataType: 'json',          
        data: data,
        contentType: 'application/json; charset=utf-8',
        async: false,
    })
    .done(function (data) {
        var waypoints = data.Waypoints;
        waypoints.forEach(point => {
            result_waypoints.push(L.latLng(point.Lat, point.Lng));
        })
        var signIds = data.SignIds;
        var maneuverIds = data.ManeuverIds;
        mapStructure.FiguresOnMap.clearLayers();
        mapStructure.FiguresOnMap.addLayer(mapStructure.StartMarker);
        mapStructure.ObjectsOnMap.forEach(obj => {
            if (obj instanceof Sign) {
                if (signIds.includes(obj.Id)) {
                    mapStructure.FiguresOnMap.addLayer(obj.Marker);
                }
            }
            else if (obj instanceof Maneuver) {
                if (maneuverIds.includes(obj.Id)) {
                    mapStructure.FiguresOnMap.addLayer(obj.StartMarker);
                    mapStructure.FiguresOnMap.addLayer(obj.EndMarker);
                }
            }
        })
    })
    .fail(function (jqxhr, textStatus, error) {
        var err = textStatus + ', ' + error;
    })

    mapStructure.APIControl = L.Routing.control({
        waypoints: result_waypoints,
        draggableWaypoints: false,
        routeWhileDragging: false,
        useZoomParameter:false,
        lineOptions: {
            addWaypoints: false
        },
        createMarker: function () { return null; }
    }).addTo(map);
    

}

