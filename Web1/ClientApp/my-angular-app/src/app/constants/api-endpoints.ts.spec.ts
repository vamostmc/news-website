import { API_ENDPOINTS } from "./api-endpoints.ts";

describe('ApiEndpoints', () => {
  it('should be defined', () => {
    expect(API_ENDPOINTS).toBeDefined();
  });

  it('should have correct API paths', () => {
    expect(API_ENDPOINTS.tinTuc.base).toContain('/TinTuc');
    expect(API_ENDPOINTS.comment.getAll).toContain('/Comment/GetAllComment');
  });
});