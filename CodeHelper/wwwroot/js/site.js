// Write your JavaScript code.
import Tags from "./tags.js";
Tags.init();

ClassicEditor
    .create(document.querySelector('#editor'), {

        licenseKey: '',

        toolbar: {
            shouldNotGroupWhenFull: true
        },

        codeBlock: {
            languages: [
                { language: "plaintext", label: "Plain text" },
                { language: "html", label: "HTML" },
                { language: "css", label: "CSS" },
                { language: "javascript", label: "JavaScript" },
                { language: "cs", label: "C#" },
                { language: "sql", label: "SQL" },
                { language: "json", label: "JSON" },
                { language: "c", label: "C" },
                { language: "cpp", label: "C++" },
                { language: "diff", label: "Diff" },
                { language: "java", label: "Java" },
                { language: "php", label: "PHP" },
                { language: "python", label: "Python" },
                { language: "ruby", label: "Ruby" },
                { language: "typescript", label: "TypeScript" },
                { language: "xml", label: "XML" }
            ],
            indentSequence: "    "
        }
    })
    .catch(error => {
        console.log(error);
    });