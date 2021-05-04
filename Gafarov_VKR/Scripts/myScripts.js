map = L.map('mapid', { editable: true }).setView([54.7370888, 55.9555806], 15);
//привязка событий карты
map.on('click', onMapClick);
map.on('zoomend', onZoomEndMap);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

if (getCookie("id") == undefined) {
    alert("Вы не имеете доступа к этой странице")
    document.location.href = "Index"
}

var USERID = Number.parseInt(getCookie("id"));

function resetPropertyEditor(found) {
    $('#propertyEditor').empty();
}
function updatePropertyEditor(found) {
    var data = found.getObjectSendForm()
    if (found instanceof Sign) {
        $.ajax({
            url: '/Home/GetSignEditForm',         /* Куда пойдет запрос */
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
            url: '/Home/GetManeuverEditForm',         /* Куда пойдет запрос */
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
        editable
    ) {
        this.UserId = userId;
        this.Editable = editable;
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
function OnSignEdit(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.Marker === e.target;
    });
    if (found.Id > 0 && !mapStructure.LoadedObjectsToChange.includes(found)) {
        mapStructure.LoadedObjectsToChange.push(found);
    }
}
function OnSignPropertyEdit(e) {
    onDifficultyLevelInput();
    var element = $("#difficultyLevel");
    var t = element.val();
    mapStructure.SelectedObject.DifficultyLevel = Number.parseInt(element.val());
    var signTypeElement = $("#elementType :selected");
    mapStructure.SelectedObject.SignTypeName = signTypeElement.val();
    if (mapStructure.SelectedObject.Id > 0 && !mapStructure.LoadedObjectsToChange.includes(mapStructure.SelectedObject)) {
        mapStructure.LoadedObjectsToChange.push(mapStructure.SelectedObject);
    }
}
class Sign extends UserInput{
    constructor(
        userId,
        editable,
        id,
        marker,
        difficultyLevel,
        signTypeName
    ) {
        super(userId, editable);
        this.Id = id;
        this.Marker = marker;
       
        this.Selected = false;
        this.MapStructure = mapStructure;
        this.Marker.setIcon(this.BlueIcon);
        this.DifficultyLevel = difficultyLevel;
        this.SignTypeName = signTypeName;
    }
    ResetSelect() {
        this.Marker.setIcon(this.BlueIcon);
        this.Selected = false;

        if (this.Editable) {
            this.Marker.off('dragend', OnSignEdit);
            this.Marker.draggable = false;
            this.Marker.dragging.disable();
        }
    }
    SetSelect() {
        this.Marker.setIcon(this.BlackIcon);
        this.Selected = true;

        if (this.Editable) {
            this.Marker.on('dragend', OnSignEdit);
            this.Marker.draggable = true;
            this.Marker.dragging.enable();
        }
    }

    getObjectSendForm() {
        var point = {
            Lat: this.Marker.getLatLng().lat,
            Lng: this.Marker.getLatLng().lng
        }
        var result = {
            Id: this.Id,
            MarkerPoint: point,
            DifficultyLevelValue: this.DifficultyLevel,
            SignTypeName: this.SignTypeName,
            UserId: this.UserId,
            Editable: this.Editable
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
function OnManeuverEdit(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.StartMarker === e.target || element.EndMarker === e.target;
    });
    if (found.Id > 0 && !mapStructure.LoadedObjectsToChange.includes(found)) {
        mapStructure.LoadedObjectsToChange.push(found);
    }
    found.Polyline.disableEdit();
    var coords = found.Polyline.getLatLngs()
    if (coords[0].lat != found.StartMarker.getLatLng().lat ||
        coords[0].lng != found.StartMarker.getLatLng().lng) {
        coords[0] = found.StartMarker.getLatLng();
    }
    if (coords[coords.length - 1].lat != found.EndMarker.getLatLng().lat ||
        coords[coords.length - 1].lng != found.EndMarker.getLatLng().lng) {
        coords[coords.length - 1] = found.EndMarker.getLatLng();
    }
    found.Polyline.setLatLngs(coords);
    found.Polyline.enableEdit();
}
function OnPolylineDrag(e) {
    e.target.deleteArrowheads();
    e.target.arrowheads();
}
function OnManeuverPolylineEdit(e) {
    var found = mapStructure.ObjectsOnMap.find((element) => {
        return element.Polyline === e.target;
    });
    if (found.Id > 0 && !mapStructure.LoadedObjectsToChange.includes(found)) {
        mapStructure.LoadedObjectsToChange.push(found);
    }
    found.Polyline.disableEdit();
    var coords = found.Polyline.getLatLngs()
    if (coords[0].lat != found.StartMarker.getLatLng().lat ||
        coords[0].lng != found.StartMarker.getLatLng().lng) {
        coords[0] = found.StartMarker.getLatLng();
    }
    if (coords[coords.length - 1].lat != found.EndMarker.getLatLng().lat ||
        coords[coords.length - 1].lng != found.EndMarker.getLatLng().lng) {
        coords[coords.length - 1] = found.EndMarker.getLatLng();
    }
    found.Polyline.setLatLngs(coords);
    found.Polyline.enableEdit();
}
function OnManeuverPropertyEdit(e) {
    onDifficultyLevelInput();
    var element = $("#difficultyLevel");
    mapStructure.SelectedObject.DifficultyLevel = Number.parseInt(element.val());
    var maneuverTypeElement = $("#elementType :selected");
    mapStructure.SelectedObject.ManeuverTypeName = maneuverTypeElement.val();
    if (mapStructure.SelectedObject.Id > 0 && !mapStructure.LoadedObjectsToChange.includes(mapStructure.SelectedObject)) {
        mapStructure.LoadedObjectsToChange.push(mapStructure.SelectedObject);
    }
}
class Maneuver extends UserInput{
    constructor(
        userId,
        editable,
        id,
        startMarker,
        endMarker,
        polyline,
        difficultyLevel,
        maneuverTypeName
    ) {
        super(userId, editable);
        this.Id = id;
        this.StartMarker = startMarker;
        this.EndMarker = endMarker;
        this.Polyline = polyline;

        this.Selected = false;
        this.MapStructure = mapStructure;
        this.StartMarker.setIcon(this.GreenIcon);
        this.EndMarker.setIcon(this.RedIcon);
        this.DifficultyLevel = difficultyLevel;
        this.ManeuverTypeName = maneuverTypeName;
    }
    ResetSelect() {
        this.StartMarker.setIcon(this.GreenIcon);
        this.EndMarker.setIcon(this.RedIcon);
        this.Polyline.deleteArrowheads();
        mapStructure.FiguresOnMap.removeLayer(this.Polyline);
        this.Selected = false;

        if (this.Editable) {
            this.StartMarker.off('dragend', OnManeuverEdit);
            this.EndMarker.off('dragend', OnManeuverEdit);
            this.Polyline.off('editable:vertex:dragend', OnManeuverPolylineEdit);
            this.Polyline.off('editable:vertex:clicked', OnManeuverPolylineEdit);
            this.Polyline.off('editable: vertex: drag', OnPolylineDrag);
            

            this.StartMarker.draggable = false;
            this.StartMarker.dragging.disable();
            this.EndMarker.draggable = false;
            this.EndMarker.dragging.disable();
            this.Polyline.disableEdit();
        }
        
    }
    SetSelect() {
        this.StartMarker.setIcon(this.BlackIcon);
        this.EndMarker.setIcon(this.BlackIcon);
        this.Polyline.arrowheads();
        mapStructure.FiguresOnMap.addLayer(this.Polyline);
        this.Selected = true;

        if (this.Editable) {
            this.StartMarker.on('dragend', OnManeuverEdit);
            this.EndMarker.on('dragend', OnManeuverEdit);
            this.Polyline.on('editable:vertex:dragend', OnManeuverPolylineEdit);
            this.Polyline.on('editable:vertex:clicked', OnManeuverPolylineEdit);
            this.Polyline.on('editable: vertex: drag', OnPolylineDrag);

            this.StartMarker.draggable = true;
            this.StartMarker.dragging.enable();
            this.EndMarker.draggable = true;
            this.EndMarker.dragging.enable();
            this.Polyline.enableEdit();
        }
        
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
        otherPoints.splice(otherPoints.length-1, 1);

        var result = {
            Id: this.Id,
            StartMarkerPoint: startPoint,
            EndMarkerPoint: endPoint,
            OtherPoints: otherPoints,
            DifficultyLevelValue: this.DifficultyLevel,
            ManeuverTypeName: this.ManeuverTypeName,
            UserId: this.UserId,
            Editable: this.Editable
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
        //объекты к удалению
        this.ObjectsToRemove = [];
        //объекты к добавлению
        this.ObjectsToAdd = [];
        //загруженные объекты с изменениями
        this.LoadedObjectsToChange = [];
        //загруженный объекты на дуаление
        this.LoadedObjectsToRemove = [];
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
    }
}
const MapStatesEnum = { "Nothing": 0, "PlaceSign": 1, "PlaceManeuver": 2, "Deleting": 3 };
Object.freeze(MapStatesEnum);
var mapStructure = new MapStructure();
LoadSignsFromDB();
LoadManeuversFromDB();

function onMapClick(e) {
    var marker = L.marker(e.latlng, { icon: mapStructure.BlackIcon });
    switch (mapStructure.MapState) {
        case MapStatesEnum.PlaceSign:
            if (mapStructure.AddedMarkers.length >= 1) {
                mapStructure.FiguresOnMap.removeLayer(mapStructure.AddedMarkers[0]);
                mapStructure.AddedMarkers.splice(0, 1);
            }
            marker.setIcon(mapStructure.BlueIcon);
            mapStructure.FiguresOnMap.addLayer(marker);
            mapStructure.AddedMarkers.push(marker);
            enableFinishSign();
            break;
        case MapStatesEnum.PlaceManeuver:
            if (mapStructure.AddedMarkers.length >= 2) {
                mapStructure.FiguresOnMap.removeLayer(mapStructure.AddedMarkers[0]);
                mapStructure.AddedMarkers.splice(0, 1);
            }
            marker.setIcon(mapStructure.GreenIcon);
            mapStructure.FiguresOnMap.addLayer(marker);
            mapStructure.AddedMarkers.push(marker);
            if (mapStructure.AddedMarkers.length == 2) {
                mapStructure.AddedMarkers[0].setIcon(mapStructure.GreenIcon);
                mapStructure.AddedMarkers[1].setIcon(mapStructure.RedIcon);
            }
            enableFinishManeuver();
            break;
    }
}
function onZoomEndMap(e) {
}

function startSign() {
    mapStructure.MapState = MapStatesEnum.PlaceSign;

    $("#signButton").attr('onclick', "finishSign()");
    $("#signButton").html("Выберите точку на карте!");
    $('#signButton').attr('disabled', 'disabled');

    $('#maneuverButton').attr('disabled', 'disabled');
    $('#deleteSelectedButton').attr('disabled', 'disabled');
    $('#saveChangesButton').attr('disabled', 'disabled');

    $("#cancelBuildButton").attr('onclick', "cancelSign()");
    $("#cancelBuildButton").removeClass('d-none');
    $("#cancelBuildButton").html("Отенить установку знака");
}
function enableFinishSign() {
    if (mapStructure.AddedMarkers.length > 0) {
        $("#signButton").html("Завершить знак");
        $('#signButton').removeAttr('disabled');
    }
}
function cancelSign() {
    mapStructure.MapState = MapStatesEnum.Nothing;
    mapStructure.AddedMarkers.forEach(marker => {
        mapStructure.FiguresOnMap.removeLayer(marker);
    });
    mapStructure.AddedMarkers = [];

    $("#signButton").attr('onclick', "startSign()");
    $('#signButton').removeAttr('disabled');
    $("#signButton").html("Указать знак");

    $('#maneuverButton').removeAttr('disabled');
    $('#deleteSelectedButton').removeAttr('disabled');
    $('#saveChangesButton').removeAttr('disabled');
    $("#cancelBuildButton").addClass('d-none');
}
function finishSign() {

    var sign = new Sign(USERID, true, 0, mapStructure.AddedMarkers[0], 1, 'Знак СТОП');
    sign.Marker.on('click', OnSignClick);
    mapStructure.ObjectsOnMap.push(sign);
    mapStructure.ObjectsToAdd.push(sign);

    mapStructure.MapState = MapStatesEnum.Nothing;
    mapStructure.AddedMarkers = [];

    $("#signButton").html("Указать знак");
    $("#signButton").attr('onclick', "startSign()");
    $('#maneuverButton').removeAttr('disabled');
    $('#deleteSelectedButton').removeAttr('disabled');
    $('#saveChangesButton').removeAttr('disabled');
    $("#cancelBuildButton").addClass('d-none');
}

function startManeuver() {
    mapStructure.MapState = MapStatesEnum.PlaceManeuver;

    $("#maneuverButton").attr('onclick', "finishManeuver()");
    $("#maneuverButton").html("Выберите 2 точки на карте!");
    $('#maneuverButton').attr('disabled', 'disabled');

    $('#signButton').attr('disabled', 'disabled');
    $('#deleteSelectedButton').attr('disabled', 'disabled');
    $('#saveChangesButton').attr('disabled', 'disabled');

    $("#cancelBuildButton").attr('onclick', "cancelManeuver()");
    $("#cancelBuildButton").html("Отенить установку манёвра");
    $("#cancelBuildButton").removeClass('d-none');


}
function enableFinishManeuver() {
    if (mapStructure.AddedMarkers.length >= 2) {
        $("#maneuverButton").html("Завершить манёвр");
        $('#maneuverButton').removeAttr('disabled');
    }
}
function cancelManeuver() {
    mapStructure.MapState = MapStatesEnum.Nothing;
    mapStructure.AddedMarkers.forEach(marker => {
        mapStructure.FiguresOnMap.removeLayer(marker);
    });
    mapStructure.AddedMarkers = [];

    $("#maneuverButton").attr('onclick', "startManeuver()");
    $('#maneuverButton').removeAttr('disabled');
    $("#maneuverButton").html("Указать манёвр");
    $('#signButton').removeAttr('disabled');
    $('#deleteSelectedButton').removeAttr('disabled');
    $('#saveChangesButton').removeAttr('disabled');
    $("#cancelBuildButton").addClass('d-none');
}
function finishManeuver() {
    coords = [];
    coords.push(mapStructure.AddedMarkers[0].getLatLng());
    coords.push(mapStructure.AddedMarkers[1].getLatLng());
    var polyline = L.polyline(coords);
    var maneuver = new Maneuver(USERID, true, 0, mapStructure.AddedMarkers[0], mapStructure.AddedMarkers[1], polyline, 1, 'Разворот');
    maneuver.StartMarker.on('click', OnManeuverClick);
    maneuver.EndMarker.on('click', OnManeuverClick);
    
    mapStructure.ObjectsOnMap.push(maneuver);
    mapStructure.ObjectsToAdd.push(maneuver);

    mapStructure.MapState = MapStatesEnum.Nothing;
    mapStructure.AddedMarkers = [];

    $("#maneuverButton").html("Указать манёвр");
    $("#maneuverButton").attr('onclick', "startManeuver()");
    $('#signButton').removeAttr('disabled');
    $('#deleteSelectedButton').removeAttr('disabled');
    $('#saveChangesButton').removeAttr('disabled');
    $("#cancelBuildButton").addClass('d-none');
}

function LoadSignsFromDB() {
    $.getJSON({
        url: '/Home/GetSignsByUserId?userId=' + USERID,
        async: false
    })
        .done(function (data) {
            data.forEach((sign) => {
                var point = L.latLng(sign.MarkerPoint.Lat, sign.MarkerPoint.Lng);
                var marker = L.marker(point, { icon: mapStructure.BlackIcon });
                var editable = sign.Editable;
                var signToLoad = new Sign(sign.UserId, editable, sign.Id, marker, sign.DifficultyLevelValue, sign.SignTypeName);
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
        url: '/Home/GetManeuversByUserId?userId=' + USERID,
    })
        .done(function (data) {
            data.forEach((maneuver) => {
                var startPoint = L.latLng(maneuver.StartMarkerPoint.Lat, maneuver.StartMarkerPoint.Lng);
                var endPoint = L.latLng(maneuver.EndMarkerPoint.Lat, maneuver.EndMarkerPoint.Lng);
                var startMarker = L.marker(startPoint, { icon: mapStructure.BlackIcon});
                var endMarker = L.marker(endPoint, { icon: mapStructure.BlackIcon });
                var coords = [];
                coords.push(startPoint);
                maneuver.OtherPoints.forEach(otherPoint => {
                    coords.push(L.latLng(otherPoint.Lat, otherPoint.Lng));
                })
                coords.push(endPoint);
                var polyline = L.polyline(coords);
                var editable = maneuver.Editable;
                var maneuverToLoad = new Maneuver(maneuver.UserId, editable, maneuver.Id, startMarker, endMarker, polyline, maneuver.DifficultyLevelValue, maneuver.ManeuverTypeName);
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
function UpdateMapContent() {
    mapStructure.FiguresOnMap.clearLayers();
    var displayType = $("#displayTypeSelector :selected").val();
    switch (displayType) {
        case "Предложенные":
            mapStructure.ObjectsOnMap.forEach(obj => {
                if (obj.Editable) {
                    if (obj instanceof Sign)
                        mapStructure.FiguresOnMap.addLayer(obj.Marker);
                    else {
                        mapStructure.FiguresOnMap.addLayer(obj.StartMarker);
                        mapStructure.FiguresOnMap.addLayer(obj.EndMarker);
                    }
                }
            });
            break;
        case "Одобренные":
            mapStructure.ObjectsOnMap.forEach(obj => {
                if (!obj.Editable) {
                    if (obj instanceof Sign)
                        mapStructure.FiguresOnMap.addLayer(obj.Marker);
                    else {
                        mapStructure.FiguresOnMap.addLayer(obj.StartMarker);
                        mapStructure.FiguresOnMap.addLayer(obj.EndMarker);
                    }
                }
            });
            break;
        case "Все":
            mapStructure.ObjectsOnMap.forEach(obj => {
                if (obj instanceof Sign)
                    mapStructure.FiguresOnMap.addLayer(obj.Marker);
                else {
                    mapStructure.FiguresOnMap.addLayer(obj.StartMarker);
                    mapStructure.FiguresOnMap.addLayer(obj.EndMarker);
                }
            });
            break;
    }
}

function selectingObjectsToDelete() {
    if (mapStructure.SelectedObject != null) {
        mapStructure.SelectedObject.ResetSelect();
        resetPropertyEditor(mapStructure.SelectedObject);
        mapStructure.SelectedObject = null;
    }

    $("#deleteSelectedButton").attr('onclick', "deleteSelectedObjects()");
    $("#deleteSelectedButton").html("Удалить выделенные объёкты");

    $('#signButton').attr('disabled', 'disabled');
    $('#maneuverButton').attr('disabled', 'disabled');
    $('#saveChangesButton').attr('disabled', 'disabled');
    $("#cancelBuildButton").attr('onclick', "cancelDelete()");
    $("#cancelBuildButton").removeClass('d-none');
    $("#cancelBuildButton").html("Отменить удаление");

    mapStructure.MapState = MapStatesEnum.Deleting;
}
function cancelDelete() {
    $("#deleteSelectedButton").attr('onclick', "selectingObjectsToDelete()");
    $("#deleteSelectedButton").html("Выделить для удаления");

    $('#signButton').removeAttr('disabled');
    $('#maneuverButton').removeAttr('disabled');
    $('#saveChangesButton').removeAttr('disabled');
    $("#cancelBuildButton").addClass('d-none');

    mapStructure.MapState = MapStatesEnum.Nothing;
    if (mapStructure.ObjectsToRemove.length >= 0) {
        mapStructure.ObjectsToRemove.forEach(obj => {
            obj.ResetSelect();
        })
        mapStructure.ObjectsToRemove = [];
    }
}
function deleteSelectedObjects() {
    if (mapStructure.ObjectsToRemove.length == 0) {
        return;
    }
    //element - ObjectOnMap
    mapStructure.ObjectsToRemove.forEach((element) => {
        //убираем с карты
        //element[1].editable.disable();
        if (element instanceof Sign)
            mapStructure.FiguresOnMap.removeLayer(element.Marker);
        else {
            mapStructure.FiguresOnMap.removeLayer(element.StartMarker);
            mapStructure.FiguresOnMap.removeLayer(element.EndMarker);
        }
        //удаляю из списка всех объектов
        var index = mapStructure.ObjectsOnMap.indexOf(element);
        if (index > -1) {
            mapStructure.ObjectsOnMap.splice(index, 1);
        }
        //удаляю из списка добавленных объектов
        index = mapStructure.ObjectsToAdd.indexOf(element);
        if (index > -1) {
            mapStructure.ObjectsToAdd.splice(index, 1);
        }

        //удаляю из списка на изменение объектов
        index = mapStructure.LoadedObjectsToChange.indexOf(element);
        if (index > -1) {
            mapStructure.LoadedObjectsToChange.splice(index, 1);
        }

        //и если этот элемент из бд, добавляю его к тем, что на удаление в бд
        if (element.Id > 0) {
            mapStructure.LoadedObjectsToRemove.push(element);
        }
    })
    cancelDelete();
}

function acceptChanges() {
    console.log("Применить изменения");
    sendObjectsToChange();
    sendObjectsToAdd();
    sendObjectsToRemove();
}

function sendSignDataToAdd(signDataToSend) {
    if (signDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/AddSigns',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(signDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
        //success: function (data) {
        //    console.log("Success, data is " + data);
        //},

        //error: function (request, status, error) {
        //    console.log("Status: " + status + "\t errpr is " + error);
        //}
    });
}
function sendManeuverDataToSend(maneuverDataToSend) {
    if (maneuverDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/AddManeuvers',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(maneuverDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
        
    });
}
function sendObjectsToAdd() {
    if (mapStructure.ObjectsToAdd.length == 0)
        return;
    var signDataToSend = [];
    var maneuverDataToSend = [];
    mapStructure.ObjectsToAdd.forEach(obj => {
        if (obj instanceof Sign) {
            signDataToSend.push(obj.getObjectSendForm());
        }
        else {
            maneuverDataToSend.push(obj.getObjectSendForm());
        }
    })
    sendSignDataToAdd(signDataToSend);
    sendManeuverDataToSend(maneuverDataToSend);
    mapStructure.ObjectsToAdd = [];
}

function sendSignDataToRemove(signDataToSend) {
    if (signDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/RemoveSigns',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(signDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
function sendManeuverDataToRemove(maneuverDataToSend) {
    if (maneuverDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/RemoveManeuvers',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(maneuverDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
function sendObjectsToRemove() {
    if (mapStructure.LoadedObjectsToRemove.length == 0)
        return;
    var signDataToSend = [];
    var maneuverDataToSend = [];
    mapStructure.LoadedObjectsToRemove.forEach(obj => {
        if (obj instanceof Sign) {
            signDataToSend.push(obj.getObjectSendForm());
        }
        else {
            maneuverDataToSend.push(obj.getObjectSendForm());
        }
    })
    sendManeuverDataToRemove(maneuverDataToSend);
    sendSignDataToRemove(signDataToSend);
    mapStructure.LoadedObjectsToRemove = [];
}

function sendSignDataToChange(signDataToSend) {
    if (signDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/ChangeSigns',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(signDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
function sendManeuverDataToChange(maneuverDataToSend) {
    if (maneuverDataToSend.length == 0)
        return;
    $.ajax({
        url: '/Home/ChangeManeuvers',         /* Куда пойдет запрос */
        method: 'post',             /* Метод передачи (post или get) */
        dataType: 'json',          /* Тип данных в ответе (xml, json, script, html). */
        data: JSON.stringify(maneuverDataToSend),
        contentType: 'application/json; charset=utf-8',
        async: false,
    });
}
function sendObjectsToChange() {
    if (mapStructure.LoadedObjectsToChange.length == 0)
        return;
    var signDataToSend = [];
    var maneuverDataToSend = [];
    mapStructure.LoadedObjectsToChange.forEach(obj => {
        if (obj instanceof Sign) {
            signDataToSend.push(obj.getObjectSendForm());
        }
        else {
            maneuverDataToSend.push(obj.getObjectSendForm());
        }
    })
    sendSignDataToChange(signDataToSend);
    sendManeuverDataToChange(maneuverDataToSend);
    mapStructure.LoadedObjectsToChange = [];
}