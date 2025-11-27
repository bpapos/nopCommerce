namespace Nop.Plugin.Payments.OpenEdge.HostPay;

public class HostPayConfig
{
	public string ServiceUrl
	{
		get
		{
			if (UseSandbox)
			{
				return "https://ee.test.paygateway.com/HostPayService/v1/directpay/express";
			}
			return "https://ee.paygateway.com/HostPayService/v1/directpay/express";
		}
	}

	public string SetupUrl
	{
		get
		{
			if (UseSandbox)
			{
				return "https://ee.test.paygateway.com/HostPayService/v1/hostpay/transactions/";
			}
			return "https://ee.paygateway.com/HostPayService/v1/hostpay/transactions/";
		}
	}

	public bool charge_type_row_visible = false;

	public bool bill_customer_title_visible = false;

	public bool bill_first_name_visible = false;

	public bool bill_middle_name_visible = false;

	public bool bill_last_name_visible = false;

	public bool bill_company_visible = false;

	public bool bill_address_one_visible = false;

	public bool bill_address_two_visible = false;

	public bool bill_city_visible = false;

	public bool bill_state_or_province_visible = false;

	public bool bill_country_code_visible = false;

	public bool bill_postal_code_visible = false;

	public bool order_information_visible = false;

	public bool charge_total_row_visible = false;

	public bool full_detail_flag = true;


	public bool UseSandbox { get; set; }

	public string font_family { get; set; }

	public string font_size { get; set; }

	public string color { get; set; }

	public string background_color { get; set; }

	public string btn_color { get; set; }

	public string btn_background_color { get; set; }

	public string btn_height { get; set; }

	public string btn_width { get; set; }

	public string btn_border_top_left_radius { get; set; }

	public string btn_border_top_right_radius { get; set; }

	public string btn_border_bottom_left_radius { get; set; }

	public string btn_border_bottom_right_radius { get; set; }

	public string btn_border_color { get; set; }

	public string btn_border_style { get; set; }

	public string btn_border_width { get; set; }

	public string btn_font_size { get; set; }

	public string section_header_font_size { get; set; }

	public string line_spacing_size { get; set; }

	public string input_field_height { get; set; }
}
