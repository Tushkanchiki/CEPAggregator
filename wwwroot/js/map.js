
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

    Init() {
        this.map = new ymaps.Map('map', START_PARAMS, {
            searchControlProvider: 'yandex#search'
        });
    }

    Add(ceps) {
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
                    balloonContent: cep.name
                }
            });
        }
        this.map.geoObjects.add(this.objectManager);
    }

}