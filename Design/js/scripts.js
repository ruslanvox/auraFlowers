// Page loading
window.addEventListener("load", () => {
    document.querySelector("body").classList.add("loaded");
});


$(document).ready(function() {
    // Menu scroll:
    $(window).on('scroll', function() {
        if ($(window).width() >= 1024) {
            if ($(window).scrollTop()) {
                $('nav').addClass('black');
            } else {
                $('nav').removeClass('black');
            }
        }
    });

    // Mobile menu:
    $(".menu-icon").on("click", function() {
        $("nav").toggleClass("showing");
        $("nav .menu").toggle();
        $("nav .social").toggle();
    });

    // Scroll for all browsers:
    $('.scroll-to').on('click', function(e) {
        e.preventDefault();
        var offset = 80;
        if ($("#navigation-section").hasClass("showing"))
            offset = 0;
        var target = this.hash;
        if ($(this).data('offset') != undefined) offset = $(this).data('offset');
        $('html, body').stop().animate({
            'scrollTop': $(target).offset().top - offset
        }, 850, 'swing', function() {
            // window.location.hash = target;
        });
    });

    // Active menu when click:
    $(".menu li a").click(function() {
        $(".menu li a").removeClass('active');
        $(this).addClass('active');

        // Auto-hide mobile menu after click on link:
        if ($("#navigation-section").hasClass("showing")) {
            $("nav").toggleClass("showing");
            $("nav .menu").toggle();
            $("nav .social").toggle();
        }
    });

    $("#getting-started-button").click(function() {
        $('#about-as-menu').addClass('active');
    });

    $(".logo a").click(function() {
        $(".menu li a").removeClass('active');
    });

    // Carousel:
    $('.owl-carousel-portfolio').owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 1
            },
            1200: {
                items: 3
            }
        },
        autowidth: true,
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true
    });

    $('.owl-carousel-testimonials').owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 1
            },
            1200: {
                items: 2
            }
        },
        autowidth: true,
        autoplay: true,
        autoplayTimeout: 10000,
        autoplayHoverPause: true
    });

    $('.play').on('click', function() {
        owl.trigger('play.owl.autoplay', [3000])
    });
    $('.stop').on('click', function() {
        owl.trigger('stop.owl.autoplay')
    });

    // Modal windows:
    $('.md-trigger').on('click', function() {
        $('.md-modal').addClass('md-show');
    });

    $('.md-close').on('click', function() {
        $('.md-modal').removeClass('md-show');
    });
});