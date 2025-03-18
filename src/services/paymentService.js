import axios from 'axios';

const API_URL = 'http://localhost:5220/api/payments';

export const addPayment = async (paymentData, token) => {
  try {
    // Debug log for the request data
    console.log('Submitting payment to backend:', paymentData);

    // Make the POST request
    const response = await axios.post(API_URL, paymentData, {
      headers: {
        Authorization: `Bearer ${token}`, // Include the JWT token
        'Content-Type': 'application/json', // Ensure the Content-Type is JSON
      },
    });

    // Debug log for the response
    console.log('Payment recorded successfully:', response.data);
    return response.data;
  } catch (error) {
    // Handle and log any errors
    console.error('Error in addPayment service:', error.response || error.message);

    // Re-throw the error to handle it in the PaymentForm or elsewhere
    throw error.response?.data?.message || 'Failed to record payment. Please try again.';
  }
};
