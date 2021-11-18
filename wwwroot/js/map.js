
const START_PARAMS = {
    center: [53.902284, 27.561831],
    zoom: 10
};

//Example
//let map = new Map();
//ymaps.ready(function () {
//    let ceps = @Html.Raw(Json.Serialize(Model));
//    console.log(ceps)
//    map.Init();
//    map.Add(ceps);
//});

class Map {

    constructor() {
    }

    async Init() {

        return ymaps.geolocation.get().then((function (res) {
            this.map = new ymaps.Map('map', {
                center: res.geoObjects.position,
                zoom: START_PARAMS.zoom
            });
            this.map.geoObjects.add(new ymaps.Placemark(res.geoObjects.position, {
                balloonContentHeader: 'Вы',
                iconContent: 'Я'
            }, {
                preset: 'islands#circleIcon',
                iconColor: 'red'
            }));
        }).bind(this), (function (e) {
            this.map = new ymaps.Map('map', START_PARAMS);
        }).bind(this));
    }

    Add(ceps, addLink) {
        this.ceps = ceps
        this.objectManager = new ymaps.ObjectManager({ clusterize: true });
        let currentId = 0;
        for (const cep of ceps) {
            this.objectManager.add({
                type: 'Feature',
                id: currentId++,
                geometry: {
                    type: 'Point',
                    coordinates: [cep.x, cep.y]
                },
                properties: {
                    hintContent: cep.bankName,
                    balloonContentHeader: cep.name,
                    balloonContentBody: addLink ? '<a href="/Cep/Index/' + cep.id + '">Больше информации</a>' : ''
                }
            });
        }
        this.map.geoObjects.add(this.objectManager);
    }

    Focus(cepId) {
        for (let cep of this.ceps) {
            if (cep.id == cepId) {
                this.map.setCenter([cep.x, cep.y]);
            }
        }
    }
}