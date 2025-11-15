export function extractApiError(err: any): string {
  if (!err) return 'Unexpected error.';

  // Our backend returns { message, errors, details }
  if (err.error?.message) return err.error.message;

  // Fluent validation errors
  if (err.error?.errors && Array.isArray(err.error.errors)) {
    return err.error.errors.join(', ');
  }

  // Default fallback
  return err.message || 'An error occurred.';
}
