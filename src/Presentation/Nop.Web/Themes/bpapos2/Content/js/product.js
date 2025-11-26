$(document).ready(function () {
  //Slick carousel
  //-----------------------------------------------
  if ($('.slick-carousel').length > 0) {
    $(".slick-carousel.carousel").slick({
      arrows: false,
      slidesToShow: 4,
      slidesToScroll: 4,
      responsive: [
        {
          breakpoint: 1200,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 992,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 2
          }
        },
        {
          breakpoint: 575,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 2
          }
        }
      ]
    });
    $(".slick-carousel.carousel-autoplay").slick({
      arrows: false,
      slidesToShow: 4,
      slidesToScroll: 4,
      autoplay: true,
      autoplaySpeed: 5000,
      responsive: [
        {
          breakpoint: 1200,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 992,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 2
          }
        },
        {
          breakpoint: 575,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 2
          }
        }
      ]
    });
    $(".slick-carousel.clients").slick({
      arrows: false,
      slidesToShow: 6,
      slidesToScroll: 6,
      autoplay: true,
      autoplaySpeed: 5000,
      responsive: [
        {
          breakpoint: 1200,
          settings: {
            slidesToShow: 6,
            slidesToScroll: 6
          }
        },
        {
          breakpoint: 992,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 4,
            slidesToScroll: 4
          }
        },
        {
          breakpoint: 575,
          settings: {
            slidesToShow: 3,
            slidesToScroll: 3
          }
        }
      ]
    });
    $(".slick-carousel.content-slider").slick({
      autoplay: true,
      autoplaySpeed: 5000,
      arrows: false
    });
    $(".slick-carousel.content-slider-with-controls").slick({
      dots: true,
      nextArrow: '<button type="button" class="slick-next">Next</button>',
      prevArrow: '<button type="button" class="slick-prev">Prev</button>'
    });
    $(".slick-carousel.content-slider-with-large-controls").slick({
      dots: true,
      nextArrow: '<button type="button" class="slick-next">Next</button>',
      prevArrow: '<button type="button" class="slick-prev">Prev</button>'
    });
    $(".slick-carousel.content-slider-with-controls-autoplay").slick({
      autoplay: true,
      autoplaySpeed: 5000,
      dots: true,
      nextArrow: '<button type="button" class="slick-next">Next</button>',
      prevArrow: '<button type="button" class="slick-prev">Prev</button>'
    });
    $(".slick-carousel.content-slider-with-large-controls-autoplay").slick({
      autoplay: true,
      autoplaySpeed: 5000,
      dots: true,
      nextArrow: '<button type="button" class="slick-next">Next</button>',
      prevArrow: '<button type="button" class="slick-prev">Prev</button>'
    });

    $('.slick-carousel.content-slider-with-thumbs').slick({
      slidesToShow: 1,
      slidesToScroll: 1,
      arrows: true,
      fade: true,
      asNavFor: '.slick-carousel.content-slider-thumbs'
    });
    $('.slick-carousel.content-slider-thumbs').slick({
      slidesToShow: 5,
      slidesToScroll: 1,
      asNavFor: '.slick-carousel.content-slider-with-thumbs',
      arrows: true,
      focusOnSelect: true
    });
  };


  // Magnific popup
  //-----------------------------------------------
  if (($(".popup-img").length > 0) || ($(".popup-iframe").length > 0) || ($(".popup-img-single").length > 0) || $(".slick-carousel--popup-img").length > 0) {
    $(".popup-img").magnificPopup({
      type: "image",
      gallery: {
        enabled: true,
      }
    });
    if ($(".slick-carousel--popup-img").length > 0) {
      $(".slick-carousel").each(function () {
        $(this).find(".slick-slide:not(.slick-cloned) .slick-carousel--popup-img").magnificPopup({
          type: "image",
          gallery: {
            enabled: true,
          }
        });
      });
    }
    $(".popup-img-single").magnificPopup({
      type: "image",
      gallery: {
        enabled: false,
      }
    });
    $('.popup-iframe').magnificPopup({
      disableOn: 700,
      type: 'iframe',
      preloader: false,
      fixedContentPos: false
    });
  }


});
