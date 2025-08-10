function previewImage(event) {
    const [file] = event.target.files;
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
        document.getElementById('profil-img').src = reader.result;
    };
    reader.readAsDataURL(file);
}

// aos library
AOS.init({
    once: true
});

// password toggle
const toggles = document.querySelectorAll('.fa-eye');
toggles.forEach(function (toggle) {
    toggle.addEventListener("click", function () {
        const input = this.closest('button').previousElementSibling;
        if (input.type === "password") {
            input.type = "text";
            this.classList.remove('fa-eye');
            this.classList.add('fa-eye-slash');
        } else {
            input.type = "password";
            this.classList.remove('fa-eye-slash');
            this.classList.add('fa-eye');
        }
    });
});