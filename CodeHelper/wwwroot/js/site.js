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
                writer.setStyle('max-height', '70px', viewEditableRoot);
                writer.setStyle('overflow', 'hidden', viewEditableRoot);
                writer.setStyle('word-wrap', 'normal', viewEditableRoot);
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
            languages: [{ language: "plaintext", label: "Code" }],
            indentSequence: "    "
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

        xhr.open('POST', 'https://localhost:7065/Questions/UploadImage', true);
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