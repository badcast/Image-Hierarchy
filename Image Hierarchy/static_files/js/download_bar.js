$(document).ready(function() {
    var dropper = $('#dropZone'),
        maxFileSize = 5000000, // максимальный размер файла - 5 мб.
        strings = {
            dropDefaultString: "Перетащите сюда файл, чтобы загрузить.",
            dropErrorString: "Не поддерживается браузером!",
            dropOverString: "Отпустите, чтобы загрузить",
            dropBigSizeString: "Файл слишком большой!",
            dropCautionIsNotAfile: "Это не файлы!",
            dropAcceptState: "Подготовка...",
            dropUploadComplete: "Файл успешно загружен!"
        },
        message = $("#message");

    if (typeof(window.FileReader) == 'undefined') {
        dropper.text(strings.dropErrorString);
        dropper.addClass('error');
        return;
    }

    var uploadState = false;

    defaultState();

    dropper[0].ondragover = function() {
        if (uploadState)
            return;
        dropper.addClass('hover');
        dropper.text(strings.dropOverString);
        return false;
    };

    dropper[0].ondragleave = function() {
        if (uploadState)
            return;
        defaultState();
        return false;
    };

    dropper[0].ondrop = async function(event) {
        if (uploadState)
            return;

        event.preventDefault();
        dropper.removeClass('hover');
        dropper.text(strings.dropAcceptState);
        if (event.dataTransfer.files.length > 0) {
            let file = event.dataTransfer.files[0];

            if (file.size > maxFileSize) {
                dropper.text(strings.dropBigSizeString);
                dropper.addClass('error');
                uploadState = true;
                setTimeout(defaultState, 1000);
                return false;
            }

            uploadState = true;

            let xhs = new XMLHttpRequest();
            xhs.addEventListener("readystatechange", (event) => {
                event.preventDefault();
                if (event.target.readyState == 4) {
                    dropper.text(strings.dropUploadComplete);
                    setTimeout(defaultState, 1000);
                }
            }, false);
            xhs.addEventListener("progress", (event) => {
                if (event.lengthComputable) {
                    var percent = Math.round(event.loaded / file.size * 100);
                    dropper.text('Uploading: ' + percent + '%');
                }
            }, false)
            xhs.open("POST", location.host + "/api/upload");
            xhs.setRequestHeader("name", encodeURIComponent(file.name));
            xhs.send(file);
            dropper.addClass('drop');
        } else {
            defaultState();
        }
    };

    function defaultState() {
        uploadState = false;
        dropper.text(strings.dropDefaultString); //стать значением по умолчанию
        dropper.removeClass('hover');
        dropper.removeClass('error');
        dropper.removeClass("drop");
    }

});