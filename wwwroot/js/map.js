
const START_PARAMS = {
    center: [53.902284, 27.561831],
    zoom: 10
};

class Map {

    constructor() {
        ymaps.ready(this.init);
    }

    init() {
        this.map = new ymaps.Map("map", START_PARAMS, {
            searchControlProvider: 'yandex#search'
        });
    }

}