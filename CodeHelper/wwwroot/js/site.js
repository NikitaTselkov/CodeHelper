// Write your JavaScript code.
import Tags from "./tags.js";
Tags.init();

var allreadOnlyNoFormatEditors = document.querySelectorAll('.readOnlyNoFormatEditor');

for (var i = 0; i < allreadOnlyNoFormatEditors.length; ++i) {
    ClassicEditor.create(allreadOnlyNoFormatEditors[i], {
        licenseKey: '',
        ui: {
            poweredBy: {
                position: 'inside',
                side: 'right',
                label: 'This is'
            }
        },
    })
        .then(editor => {
            editor.execute('removeFormat');
            editor.enableReadOnlyMode('readOnlyNoFormatEditor');
            editor.ui.view.toolbar.element.style.display = 'none';
            editor.editing.view.change(writer => {
                const viewEditableRoot = editor.editing.view.document.getRoot();
                writer.removeClass('ck-editor__editable_inline', viewEditableRoot);
                writer.removeClass('ck-editor__editable', viewEditableRoot);
                writer.setStyle('max-height', '70px', viewEditableRoot);
                writer.setStyle('overflow', 'hidden', viewEditableRoot);
                writer.setStyle('word-wrap', 'normal', viewEditableRoot);

                $('br').remove();
                $("p").each(function () {
                    if ($.trim($(this).text()) == "") {
                        $(this).remove();
                    }
                });
            });
        })
        .catch(error => {
            console.log(error);
        });
}

var allReadOnlyEditors = document.querySelectorAll('.readOnlyEditor');

for (var i = 0; i < allReadOnlyEditors.length; ++i) {
    ClassicEditor.create(allReadOnlyEditors[i], {
        licenseKey: '',
        toolbar: [],
        ui: {
            poweredBy: {
                position: 'inside',
                side: 'right',
                label: 'This is'
            }
        },
    })
        .then(editor => {
            window.editor = editor;
            editor.enableReadOnlyMode('readOnlyEditor');
            editor.ui.view.toolbar.element.style.display = 'none';
            editor.editing.view.change(writer => {
                const viewEditableRoot = editor.editing.view.document.getRoot();
                writer.removeClass('ck-editor__editable_inline', viewEditableRoot);

                $('p').find('br').remove();
                $("p").each(function () {
                    if ($.trim($(this).text()) == "") {
                        $(this).remove();
                    }
                });
            });
        })
        .catch(error => {
            console.log(error);
        });
}

var allEditors = document.querySelectorAll('.editor');

for (var i = 0; i < allEditors.length; ++i) {
    ClassicEditor.create(allEditors[i], {
        extraPlugins: [MyCustomUploadAdapterPlugin],
        licenseKey: '',
        ui: {
            poweredBy: {
                position: 'inside',
                side: 'right',
                label: 'This is'
            }
        },
        toolbar: {
            shouldNotGroupWhenFull: true
        },
        codeBlock: {
            languages: [{ language: "plaintext", label: "Code" }]
        }
    })
        .catch(error => {
            console.log(error);
        });
}

class MyUploadAdapter {
    constructor(loader) {
        // The file loader instance to use during the upload.
        this.loader = loader;
    }

    // Starts the upload process.
    upload() {
        return this.loader.file
            .then(file => new Promise((resolve, reject) => {
                this._initRequest();
                this._initListeners(resolve, reject, file);
                this._sendRequest(file);
            }));
    }

    // Aborts the upload process.
    abort() {
        if (this.xhr) {
            this.xhr.abort();
        }
    }

    // Initializes the XMLHttpRequest object using the URL passed to the constructor.
    _initRequest() {
        const xhr = this.xhr = new XMLHttpRequest();

        xhr.open('POST', 'https://rewritecode.ru/Questions/UploadImage', true);
        xhr.responseType = 'json';
    }

    // Initializes XMLHttpRequest listeners.
    _initListeners(resolve, reject, file) {
        const xhr = this.xhr;
        const loader = this.loader;
        const genericErrorText = `Couldn't upload file: ${file.name}.`;

        xhr.addEventListener('error', () => reject(genericErrorText));
        xhr.addEventListener('abort', () => reject());
        xhr.addEventListener('load', () => {
            const response = xhr.response;

            if (!response || response.error) {
                return reject(response && response.error ? response.error.message : genericErrorText);
            }

            resolve({
                default: response.url
            });
        });

        if (xhr.upload) {
            xhr.upload.addEventListener('progress', evt => {
                if (evt.lengthComputable) {
                    loader.uploadTotal = evt.total;
                    loader.uploaded = evt.loaded;
                }
            });
        }
    }

    // Prepares the data and sends the request.
    _sendRequest(file) {
        const data = new FormData();

        data.append('files', file);

        this.xhr.send(data);
    }
}
function MyCustomUploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
        return new MyUploadAdapter(loader);
    };
}

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        let date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    let matches = document.cookie.match(new RegExp("(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

function checkCookies() {
    let cookieNote = document.getElementById('cookie_note');
    let cookieBtnAccept = cookieNote.querySelector('.cookie_accept');

    // Если куки cookies_policy нет или она просрочена, то показываем уведомление
    if (!getCookie('cookies_policy')) {
        cookieNote.classList.add('show');
    }

    // При клике на кнопку устанавливаем куку cookies_policy на один год
    cookieBtnAccept.addEventListener('click', function () {
        setCookie('cookies_policy', 'true', 30);
        cookieNote.classList.remove('show');
    });
}

checkCookies();