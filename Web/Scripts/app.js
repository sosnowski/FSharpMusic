$(document).ready(function () {
    var form = $('.composer form');
    var field = form.find('textarea');

    var tones = {
        barbie: '8#g2 8e2 8#g2 8#c3 4a2 4- 8#f2 8#d2 8#f2 8b2 4#g2 8#f2 8e2 4- 8e2 8#c2 4#f2 4#c2 4- 8#f2 8e2 4#g2 4#f2',
        metal: '4e1 8e1 8e2 8g2 8#a1 4a1 8e2 4e1 8e1 8e2 8g2 8#a1 4a1 8e2 4e1',
        starwars: '4e1 4e1 4e1 8c1 16- 16g1 4e1 8c1 16- 16g1 4e1 4- 4b1 4b1 4b1 8c2 16- 16g1 4#d1 8c1 16- 16g1 4e1 8-',
        tiger: '8d1 8e1 8f1 8- 8f1 16f1 8f1 16- 8e1 8d1 8c1 8c1 8d1 8e1 8d1 8- 8d1 8e1 8f1 16- 32- 8e1 8f1 8g1 16- 32- 8f1 16- 32a1 8- 16- 2a1 4- 8d1 16c1 8d1 16- 8c1'
    }

    var loadFile = function (toneValue, callback) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/home/generate', true);
        xhr.responseType = 'arraybuffer';

        xhr.onload = function () {
            if (this.status === 200) {
                var type = xhr.getResponseHeader('Content-Type');
                callback(null, new Blob([this.response], { type: type }));
            } else {
                callback(new Error('Request error'));
            }
        };


        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send($.param({
            score: toneValue
        }));
    };

    var displayAudio = function (code, audioUrl) {
        $('#player').html('<h3>Play</h3> <p>' + code + '</p><audio src="' + audioUrl + '" controls="controls" autoplay>Audio is not available</audio>');
    };

    form.on('submit', function (e) {
        e.preventDefault();
        var val = field.val();
        loadFile(val, function (err, res) {
            if (err) {
                throw err;
            }
            var fileUrl = window.URL.createObjectURL(res);
            console.log(fileUrl);
            displayAudio(val, fileUrl);
        })
    });

    $('.menu ul a').on('click', function (e) {
        e.preventDefault();
        field.val(tones[this.title]);
    });
});