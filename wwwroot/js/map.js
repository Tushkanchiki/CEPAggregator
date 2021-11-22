
const START_PARAMS = {
    center: [53.902284, 27.561831],
    zoom: 10,
    controls: ['routePanelControl']
};

const ROUTE_PARAMS = {
    reverseGeocoding: true
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
                zoom: START_PARAMS.zoom,
                controls: START_PARAMS.controls
            });
            this.map.geoObjects.add(new ymaps.Placemark(res.geoObjects.position, {
                balloonContentHeader: 'Вы',
                iconContent: 'Я'
            }, {
                preset: 'islands#circleIcon',
                iconColor: 'red'
            }));

            let control = this.map.controls.get('routePanelControl');
            control.routePanel.state.set({
                fromEnabled: true,
                from: res.geoObjects.position[0] + ', ' + res.geoObjects.position[1],
                toEnabled: true
            });
            control.routePanel.options.set(ROUTE_PARAMS);
            control.options.set({
               autofocus: false
            });
        }).bind(this), (function (e) {
            this.map = new ymaps.Map('map', START_PARAMS);
            let control = this.map.controls.get('routePanelControl');
            control.routePanel.options.set(ROUTE_PARAMS);
            control.options.set({
                autofocus: false
            });
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
                    balloonContentBody: addLink ? '<a href="/Cep/Index/' + cep.id + '">Больше информации</a>' : '',
                }
            });
        }
        this.objectManager.objects.events.add('click', function (e) {
            let objectId = e.get('objectId');
            this.objectManager.objects.balloon.open(objectId);
            this.RouteTo(this.ceps[objectId].id);
        }.bind(this));
        this.map.geoObjects.add(this.objectManager);
    }

    Focus(cepId) {
        for (let cep of this.ceps) {
            if (cep.id == cepId) {
                this.map.setCenter([cep.x, cep.y]);
                this.map.setZoom(18);
            }
        }
    }

    RouteTo(cepId) {
        for (let cep of this.ceps) {
            if (cep.id == cepId) {
                let control = this.map.controls.get('routePanelControl');
                control.routePanel.state.set({
                    to: cep.x + ', ' + cep.y
                });
            }
        }
    }
}